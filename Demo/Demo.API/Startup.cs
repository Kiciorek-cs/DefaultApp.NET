using System;
using Asp.Versioning.ApiExplorer;
using Demo.API.BackgroundServices;
using Demo.API.Extensions;
using Demo.API.Middlewares;
using Demo.API.PipelinesBehaviours;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Services.Clock;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Serilog;
using Swashbuckle.AspNetCore.SwaggerUI;

namespace Demo.API;

public class Startup
{
    private readonly string _informationWhereMediatorWasImplemented = "Demo.BLL";

    public IClock Clock = new UtcClock();

    public Startup(IConfiguration configuration)
    {
        Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
        services.AddControllers().AddNewtonsoftJson(jsonOptions =>
        {
            jsonOptions.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
            jsonOptions.SerializerSettings.Converters.Add(new StringEnumConverter());
        });

        // Pipelines
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(LoggingBehaviour<,>));

        // SWAGGER 
        this.ConfigureSwaggerInjection(services, Configuration);

        // SERVICES
        this.ConfigureServicesInjection(services, Clock);

        CreateLogger(Configuration, services.BuildServiceProvider());

        // AppSettings
        services.ConfigureSettings(Configuration);

        // AutoMapper
        services.ConfigureAutoMapperInjection();

        // AddAuthentication
        //services.AddJwtAuthentication();

        // CustomAuthorization
        services.AddCustomAuthorization(Configuration);

        // Database connection
        this.ConfigureDbInjection(services, Clock);


        // MediatR
        services.AddValidatorsFromAssembly(AppDomain.CurrentDomain.Load(_informationWhereMediatorWasImplemented));
        services.AddMediatR(cfg =>
            cfg.RegisterServicesFromAssemblies(AppDomain.CurrentDomain.Load(_informationWhereMediatorWasImplemented)));
        services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

        services.AddSingleton<IHostedService, StateObserver>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
        if (env.IsDevelopment()) app.UseDeveloperExceptionPage();

        app.UseMiddleware<TraceIdMiddleware>();
        app.UseMiddleware<RequestLoggingMiddleware>();

        //Add support to logging request with SERILOG
        app.UseSerilogRequestLogging();

        app.UseStaticFiles();

        app.UsePathBase("/Demo");

        app.UseSwagger();

        var apiVersionDescriptionProvider =
            app.ApplicationServices.GetRequiredService<IApiVersionDescriptionProvider>();

        app.UseSwaggerUI(c =>
        {
            foreach (var description in apiVersionDescriptionProvider.ApiVersionDescriptions)
                c.SwaggerEndpoint($"../swagger/{description.GroupName}/swagger.json",
                    description.GroupName.ToUpperInvariant());

            c.InjectStylesheet("../swaggersStyles/SwaggerDark.css");
            c.DocExpansion(DocExpansion.None);
            c.RoutePrefix = "api";
        });

        app.UseCors(builder => builder
            .AllowAnyOrigin()
            .AllowAnyMethod()
            .AllowAnyHeader());

        app.UseHttpsRedirection();

        app.UseRouting();

        app.UseAuthorization();

        app.UseAuthentication();

        app.UseCustomExceptionHandler();

        app.UseEndpoints(endpoints => { endpoints.MapControllers(); });
    }

    private static void CreateLogger(IConfiguration configuration, ServiceProvider buildServiceProvider)
    {
        Log.Logger = new LoggerConfiguration()
            //.Enrich.FromLogContext()
            //.Enrich.WithMachineName()
            //.ReadFrom.Configuration(configuration)
            .WriteTo.Console()
            .WriteTo.Sink(new CustomDatabaseSink(configuration.GetConnectionStringToDemoDb(),
                buildServiceProvider.GetRequiredService<IHttpContextAccessor>()))
            .CreateLogger();
    }
}