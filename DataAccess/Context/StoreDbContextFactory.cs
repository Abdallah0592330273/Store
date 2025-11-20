//using Microsoft.EntityFrameworkCore;
//using Microsoft.EntityFrameworkCore.Design;
//using Microsoft.EntityFrameworkCore.Infrastructure;
//using Microsoft.Extensions.Configuration;
//using System;
//using System.Collections.Generic;
//using System.IO;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Threading.Tasks;




//namespace DataAccess.Context
//{
//    public class StoreDbContextFactory : IDesignTimeDbContextFactory<StoreDbContext>
//    {


//        public StoreDbContext CreateDbContext(string[] args)
//        {
//            // Manually read appsettings.json
//            var appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "appsettings.json");

//            if (!File.Exists(appSettingsPath))
//            {
//                // Try going up one level (common in class library projects)
//                appSettingsPath = Path.Combine(Directory.GetCurrentDirectory(), "..", "Your.Web.Project", "appsettings.json");
//            }

//            var json = File.ReadAllText(appSettingsPath);
//            var jsonDocument = JsonDocument.Parse(json);
//            var connectionString = jsonDocument.RootElement
//                .GetProperty("ConnectionStrings")
//                .GetProperty("DefaultConnection")
//                .GetString();

//            var optionsBuilder = new DbContextOptionsBuilder<StoreDbContext>();
//            optionsBuilder.UseSqlServer(connectionString);

//            return new StoreDbContext(optionsBuilder.Options);
//        }
//    }
//    }
