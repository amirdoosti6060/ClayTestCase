using Microsoft.EntityFrameworkCore;
using DoorWebAPI.Models;

namespace DoorWebAPI.Helpers
{
    public static class DatabaseInitializerExtention
    {
        private static void InitiateTables(DoorDbContext dbContext)
        {
            List<Door> doors = new List<Door>
            {
                new Door
                {
                    Name = "Main Entrance",
                    HardwareId = "o-main",
                    ModifiedAt = DateTime.Now
                },
                new Door
                {
                    Name = "Storage Room",
                    HardwareId = "o-storage",
                    ModifiedAt = DateTime.Now
                },
            };

            List<Permission> permissions = new List<Permission>
            {
                new Permission { DoorId = 1, Role = "administrator" },
                new Permission { DoorId = 1, Role = "manager" },
                new Permission { DoorId = 1, Role = "employee" },
                new Permission { DoorId = 2, Role = "administrator" },
                new Permission { DoorId = 2, Role = "manager" }
            };

            if (!dbContext.Doors.Any())
            {
                dbContext.Doors.AddRange(doors);
                dbContext.SaveChanges();
            }

            if (!dbContext.Permissions.Any())
            {
                dbContext.Permissions.AddRange(permissions);
                dbContext.SaveChanges();
            }
        }

        public static void InitiateDatabase(this WebApplication app)
        {
            using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<DoorDbContext>();
                var db = dbContext.Database;
                bool connected = false;

                while (!connected)
                {
                    try
                    {
                        connected = db.CanConnect();
                        if (!connected)
                        {
                            logger.LogInformation("Migrating database...");
                            db.Migrate();
                        }
                    }
                    catch (Exception ex)
                    {
                        logger.LogError("Unable to migrate the database!");
                        logger.LogError(ex.Message);
                        logger.LogInformation("Database not ready yet; waiting...");
                        Thread.Sleep(1000);
                        connected = false;
                    }
                }

                InitiateTables(dbContext);
                logger.LogInformation("Database migrated successfully.");
            }
        }
    }
}
