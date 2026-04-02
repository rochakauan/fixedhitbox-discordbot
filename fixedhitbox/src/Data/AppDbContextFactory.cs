using DotNetEnv;
using fixedhitbox.config;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace fixedhitbox.Data;

public class AppDbContextFactory : IDesignTimeDbContextFactory<AppDbContext>
{

    public AppDbContext CreateDbContext(string[] args)
    {
        Env.Load();

        var conf = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddEnvironmentVariables()
            .Build();

        var options = new DbContextOptionsBuilder<AppDbContext>();
        options.UseSqlite(DbConfig.ResolveConnectionString(conf));

        return new AppDbContext(options.Options);
    }
}