using DoorWebAPI.Models;

namespace DoorWebAPI.Test.Systems.MockData
{
    public static class PermissionMockData
    {
        public static List<Permission> GetPermissions()
        {
            return new List<Permission>()
            {
                new Permission() { Id = 1, DoorId = 1, Role = "administrator"},
                new Permission() { Id = 2, DoorId = 1, Role = "manager"},
                new Permission() { Id = 3, DoorId = 1, Role = "employee"},
                new Permission() { Id = 4, DoorId = 2, Role = "manager"}
            };
        }
    }
}
