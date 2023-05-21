using Microsoft.EntityFrameworkCore;
using UserWebAPI.Models;

namespace UserWebAPI.Helpers
{
    public static class DatabaseInitializerExtention
    {
        private static void InitiateTables(UserDbContext dbContext)
        {
            List<User> users = new List<User>
            {
                new User
                {
                    Email = "admin@test.com",
                    Password = "1234",
                    FullName = "Administrator",
                    Role = "administrator",
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Email = "manager@test.com",
                    Password = "5678",
                    FullName = "Manager",
                    Role = "manager",
                    CreatedAt = DateTime.Now
                },
                new User
                {
                    Email = "employee@test.com",
                    Password = "1111",
                    FullName = "User01",
                    Role = "employee",
                    CreatedAt = DateTime.Now
                }
            };

            if (!dbContext.Users.Any())
            {
                dbContext.Users.AddRange(users);
                dbContext.SaveChanges();
            }
        }

        public static void InitiateDatabase(this WebApplication app)
        {
            using (var serviceScope = app.Services.GetRequiredService<IServiceScopeFactory>().CreateScope())
            {
                var logger = serviceScope.ServiceProvider.GetRequiredService<ILogger<Program>>();
                var dbContext = serviceScope.ServiceProvider.GetRequiredService<UserDbContext>();
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
