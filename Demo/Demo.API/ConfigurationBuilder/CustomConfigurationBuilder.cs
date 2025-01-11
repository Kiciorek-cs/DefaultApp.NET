using System;
using System.Configuration;
using Demo.API.ConfigurationBuilder.Models;
using Demo.BLL.Interfaces.Services.Clock;
using Demo.BLL.Services.ErrorLogger;

namespace Demo.API.ConfigurationBuilder;

//These methods will read the configuration from the web.config file
public class CustomConfigurationBuilder
{
    private const string ConfigurationProblemMessage =
        "Nie można kontynuować działania aplikacji z powodu problemów z konfiguracją.";

    private static string ConnectionStringTemplate(string host, string port, string databaseName, string login,
        string password)
    {
        return
            $"server={host},{port}; database={databaseName}; User Id={login}; Password={password}; Integrated Security=False;Persist Security Info=False; TrustServerCertificate=True";
    }

    // you can add a static field with config and initialize only if it is null, then you can do reading once and loading later

    public static string CreateDatabaseConfiguration(IClock clock)
    {
        try
        {
            var configuration = ReadWebConfigFile(clock);

            var host = configuration.AppSettings.Settings["Host"];
            var port = configuration.AppSettings.Settings["Port"];
            var databaseName = configuration.AppSettings.Settings["DatabaseName"];
            var login = configuration.AppSettings.Settings["Login"];
            var password = configuration.AppSettings.Settings["Password"];


            CheckIfPropertiesAreNotNull(
                clock,
                new SectionValidationModel[]
                {
                    new("Host", host),
                    new("Port", port),
                    new("DatabaseName", databaseName),
                    new("Login", login),
                    new("Password", password)
                }
            );

            CheckIfPropertiesAreNotEmpty(
                clock,
                new SectionValidationModel[]
                {
                    new("Host", host),
                    new("Port", port),
                    new("DatabaseName", databaseName),
                    new("Login", login),
                    new("Password", password)
                }
            );

            return ConnectionStringTemplate(host.Value, port.Value, databaseName.Value, login.Value, password.Value);
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(
                clock.Current(),
                $"Something is wrong with the configuration: {ex.Message}");

            FailFast();

            throw;
        }
    }

    public static string GetLogsPath(IClock clock)
    {
        try
        {
            var configuration = ReadWebConfigFile(clock);

            var logsPath = configuration.AppSettings.Settings["LogsPath"];

            CheckIfPropertiesAreNotNull(
                clock,
                new SectionValidationModel[]
                {
                    new("LogsPath", logsPath)
                }
            );

            CheckIfPropertiesAreNotEmpty(
                clock,
                new SectionValidationModel[]
                {
                    new("LogsPath", logsPath)
                }
            );

            return logsPath.Value;
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(
                clock.Current(),
                $"Something is wrong with the configuration: {ex.Message}");

            FailFast();

            throw;
        }
    }

    private static Configuration ReadWebConfigFile(IClock clock)
    {
        try
        {
            var fileMap = new ExeConfigurationFileMap { ExeConfigFilename = @"web.config" };
            var configuration =
                ConfigurationManager.OpenMappedExeConfiguration(fileMap, ConfigurationUserLevel.None);

            if (!configuration.HasFile)
                throw new Exception("web.config file does not exist");

            return configuration;
        }
        catch (Exception ex)
        {
            ErrorLogger.LogError(clock.Current(),
                $"Problem with web.config file: {ex.Message}");

            FailFast();

            throw;
        }
    }

    private static void CheckIfPropertiesAreNotNull(IClock clock, params SectionValidationModel[] elements)
    {
        foreach (var element in elements)
            if (element.KeyValueConfigurationElement is null)
            {
                ErrorLogger.LogError(clock.Current(),
                    $"Section '{element.Name}' is not exist.");

                FailFast();
            }
    }

    private static void CheckIfPropertiesAreNotEmpty(IClock clock, params SectionValidationModel[] elements)
    {
        foreach (var element in elements)
            if (string.IsNullOrEmpty(element.KeyValueConfigurationElement.Value))
            {
                ErrorLogger.LogError(clock.Current(),
                    $"Section '{element.Name}' is empty.");

                FailFast();
            }
    }

    private static void FailFast()
    {
        Environment.FailFast(ConfigurationProblemMessage);
    }
}