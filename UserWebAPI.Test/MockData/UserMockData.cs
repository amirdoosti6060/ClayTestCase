using UserWebAPI.Models;

namespace UserWebAPI.Test.Systems.MockData
{
    public static class UserMockData
    {
        public static List<User> GetUsers()
        {
            return new List<User>()
            {
                new User() { Id = 1, Email = "admin@test.com", Password = "123", FullName = "Administrator", Role = "administrator", CreatedAt = DateTime.Now },
                new User() { Id = 2, Email = "user@gmail.com", Password = "456", FullName = "Manger", Role = "manager", CreatedAt = DateTime.Now },
                new User() { Id = 3, Email = "manager@gmail.com", Password = "789", FullName = "Employee", Role = "user", CreatedAt = DateTime.Now }
            };
        }
    }
}
