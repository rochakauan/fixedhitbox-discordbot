using Microsoft.Extensions.Configuration;

namespace fixedhitbox.config;

public static class DbConfig
{
    public static string ResolveConnectionString(IConfiguration config)
    {
        var configuredPath = config["DB__Path"];

        var dbPath = string.IsNullOrWhiteSpace(configuredPath)
            ? Path.Combine(Directory.GetCurrentDirectory(),
            "database", "fhbot.db")
            : configuredPath;

        Directory.CreateDirectory(Path.GetDirectoryName(dbPath)!);

        return config.GetConnectionString("DefaultConnection")
            ?? config["DB__Connection"]
            ?? $"Data Source={dbPath}";
    }
}