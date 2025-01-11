using System;
using System.IO;
using Demo.Domain.ConfigurationModels.StaticModels;

namespace Demo.BLL.Services.ErrorLogger;

public static class ErrorLogger
{
    private static readonly object lockObject = new();

    public static void LogError(DateTimeOffset date, string errorMessage)
    {
        var folderPath = string.IsNullOrEmpty(ErrorCustomConfigurationModel.LogsPath)
            ? "Logs"
            : ErrorCustomConfigurationModel.LogsPath;

        var path = $"{folderPath}";
        var fileName = $"Errors_{date:yyyy-MM-dd}.log";

        var fullPath = Path.Combine(path, fileName);

        lock (lockObject)
        {
            if (!Directory.Exists(path))
                Directory.CreateDirectory(path);

            if (!File.Exists(fullPath))
                File.Create(fullPath).Dispose();

            using (var writer = new StreamWriter(fullPath, true))
            {
                writer.WriteLine("\n" + date.ToString("yyyy-MM-dd HH:mm:ss") + " - " + errorMessage);
            }
        }
    }
}