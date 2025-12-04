using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;
namespace Store.DataAccess.Context;

public class StoreContextFactory : IDesignTimeDbContextFactory<StoreContext>
{
    public StoreContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<StoreContext>();

        // 1. Define the path to the startup project where appsettings.json lives.
        // NOTE: This assumes InterviewPass.WebApi is the startup project and is 
        // one level up from the DataAccess project, which is common.
        var startupProjectPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "InterviewPass.WebApi");

        // Fallback for different project structures or if run from the solution root
        if (!Directory.Exists(startupProjectPath))
        {
            startupProjectPath = Directory.GetCurrentDirectory();
        }

        // 2. Build a configuration object to read appsettings.json
        IConfigurationRoot configuration = new ConfigurationBuilder()
            // Set the base path to the directory containing appsettings.json
            .SetBasePath(startupProjectPath)
            // Use AddJsonFile to load configuration from the correct file, making it optional
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile($"appsettings.Development.json", optional: true) // Good practice to include environment
            .Build();

        // 3. Load and validate the connection string
        var connectionString = configuration.GetConnectionString("DefaultConnection");

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                "Could not find a connection string named 'DefaultConnection'. " +
                "Please ensure appsettings.json is copied to the startup directory and contains the connection string."
            );
        }

        // 4. Apply options and return context
        optionsBuilder.UseSqlServer(connectionString);

        return new StoreContext(optionsBuilder.Options);
    }
}