using Asp.Versioning;
using Asp.Versioning.ApiExplorer;
using Demo.Domain.ConfigurationModels.DynamicModel;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace Demo.API.Extensions;

public static class SwaggerInjectionExtension
{
    public static void ConfigureSwaggerInjection(this Startup startup, IServiceCollection services,
        IConfiguration configuration)
    {
        var jwtTokenSettings = configuration.GetSection("JwtTokenSettings").Get<JwtTokenSettings>();

        var headerTokenName = jwtTokenSettings.HeaderName;

        services.AddEndpointsApiExplorer();

        services.AddSwaggerGen(c =>
        {
            c.AddSecurityDefinition(headerTokenName, new OpenApiSecurityScheme
            {
                In = ParameterLocation.Header,
                Name = headerTokenName,
                Type = SecuritySchemeType.ApiKey,
                Description =
                    "JWT Authorization header using the Bearer scheme. \r\n\r\n Enter 'Bearer' [space] and then your token in the text input below.\r\n\r\nExample: \"Bearer 12345abcdef\"",
                Scheme = "Bearer",
                BearerFormat = "JWT"
            });

            c.AddSecurityRequirement(new OpenApiSecurityRequirement
            {
                {
                    new OpenApiSecurityScheme
                    {
                        Name = headerTokenName,
                        Type = SecuritySchemeType.ApiKey,
                        In = ParameterLocation.Header,
                        Reference = new OpenApiReference
                        {
                            Type = ReferenceType.SecurityScheme,
                            Id = headerTokenName
                        }
                    },
                    new string[] { }
                }
            });
        });

        services.AddApiVersioning(opt =>
        {
            opt.DefaultApiVersion = new ApiVersion(1, 0);
            opt.AssumeDefaultVersionWhenUnspecified = true;
            opt.ReportApiVersions = true;
            opt.ApiVersionReader = ApiVersionReader.Combine(new UrlSegmentApiVersionReader(),
                new HeaderApiVersionReader("x-api-version"),
                new MediaTypeApiVersionReader("x-api-version"));
        }).AddApiExplorer(setup =>
        {
            setup.GroupNameFormat = "'v'VVV";
            setup.SubstituteApiVersionInUrl = true;
        });

        services.ConfigureOptions<ConfigureSwaggerOptions>();
    }
}

public class ConfigureSwaggerOptions
    : IConfigureNamedOptions<SwaggerGenOptions>
{
    private readonly IApiVersionDescriptionProvider _provider;

    public ConfigureSwaggerOptions(
        IApiVersionDescriptionProvider provider)
    {
        _provider = provider;
    }

    /// <summary>
    ///     Configure each API discovered for Swagger Documentation
    /// </summary>
    /// <param name="options"></param>
    public void Configure(SwaggerGenOptions options)
    {
        // add swagger document for every API version discovered
        foreach (var description in _provider.ApiVersionDescriptions)
            options.SwaggerDoc(
                description.GroupName,
                CreateVersionInfo(description));
    }

    /// <summary>
    ///     Configure Swagger Options. Inherited from the Interface
    /// </summary>
    /// <param name="name"></param>
    /// <param name="options"></param>
    public void Configure(string name, SwaggerGenOptions options)
    {
        Configure(options);
    }

    /// <summary>
    ///     Create information about the version of the API
    /// </summary>
    /// <param name="desc"></param>
    /// <returns>Information about the API</returns>
    private OpenApiInfo CreateVersionInfo(
        ApiVersionDescription desc)
    {
        var info = new OpenApiInfo
        {
            Title = "Demo.API",
            Version = desc.ApiVersion.ToString(),
            Contact = new OpenApiContact
                { Name = "UnKnow" }
        };

        if (desc.IsDeprecated)
            info.Description +=
                " This API version has been deprecated. Please use one of the new APIs available from the explorer.";

        return info;
    }
}