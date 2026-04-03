using Microsoft.Extensions.Configuration;

namespace fixedhitbox.config;

public static class DbConfig
{
    public static string ResolveConnectionString(IConfiguration config)
    {
        var connectionString = config.GetConnectionString("DefaultConnection")
            ?? config["DB__Connection"];
        
        if (!string.IsNullOrWhiteSpace(connectionString))
            return connectionString;
        
        var dbPath = config["DB__Path"];

        if (string.IsNullOrWhiteSpace(dbPath))
        {
            var baseDir = AppContext.BaseDirectory;

            if (baseDir.Contains($"{Path.DirectorySeparatorChar}bin{Path.DirectorySeparatorChar}"))
                dbPath = Path.GetFullPath(Path.Combine(baseDir,
                    "..", "..", "..", "database", "fhbot.db"));
                    //bin/Debug/netX/...
            else
                dbPath = Path.Combine(baseDir, "database", "fhbot.db");
        } 
        
        var dir = Path.GetDirectoryName(dbPath);
        if (!string.IsNullOrEmpty(dir))
            Directory.CreateDirectory(dir);
                   
        return $"Data Source={dbPath}";
    }
}