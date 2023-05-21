using Microsoft.EntityFrameworkCore;
using HistoryWebAPI.Models;

namespace HistoryWebAPI.Helpers
{
    public static class DatabaseInitializerExtention
    {
        private static void InitiateTables(HistoryDbContext dbContext)
        {
            List<History> histories = new List<History>
            {
                new History
                {
                    DoorId = 1,
                    DoorName = "Main Entrance",
                    HardwareId = "o-main",
                    UserId = 1,
                    FullName = "Administrator",
                    Email = "admin@test.com",
                    Role = "administrator",
                    ActionStatus = "ok",
                    TimeStamp = DateTime.Now
                }
            };

            if (!dbContext.History.Any())
            {
                dbContext.History.AddRange(histories);
                dbContext.SaveChanges();
            }
        }

        public static void InitiateDatabase(this WebApplication app)
        {
            using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<HistoryDbContext>();
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
