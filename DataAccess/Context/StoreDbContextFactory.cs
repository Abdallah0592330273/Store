using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using System.IO;
using System.Text.Json;

namespace DataAccess.Context
{
    public class StoreDbContextFactory : IDesignTimeDbContextFactory<StoreDbContext>
    {
        public StoreDbContext CreateDbContext(string[] args)
        {
            var currentDir = Directory.GetCurrentDirectory();
            Console.WriteLine($"Current Directory: {currentDir}");

            // Find appsettings.json from the web project
            var appSettingsPath = Path.Combine(currentDir, "appsettings.json");

            if (!File.Exists(appSettingsPath))
            {
                // Try going up to find it in the web project
                appSettingsPath = Path.Combine(currentDir, "..", "Store.web", "appsettings.json");
                Console.WriteLine($"Trying path: {Path.GetFullPath(appSettingsPath)}");
            }

            if (!File.Exists(appSettingsPath))
            {
                throw new FileNotFoundException($"appsettings.json not found at: {Path.GetFullPath(appSettingsPath)}");
            }

            var json = File.ReadAllText(appSettingsPath);
            var jsonDocument = JsonDocument.Parse(json);
            var connectionString = jsonDocument.RootElement
                .GetProperty("ConnectionStrings")
                .GetProperty("DefaultConnection")
                .GetString();

            var optionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
            optionsBuilder.UseSqlServer(connectionString);

            return new StoreDbContext(optionsBuilder.Options);
        }
    }
}