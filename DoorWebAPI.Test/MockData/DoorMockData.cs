using DoorWebAPI.Models;

namespace DoorWebAPI.Test.Systems.MockData
{
    public static class DoorMockData
    {
        public static List<Door> GetDoors()
        {
            return new List<Door>()
            {
                new Door() { Id = 1, Name = "Main Enterance", HardwareId = "o-main", ModifiedAt = DateTime.Now },
                new Door() { Id = 2, Name = "Storage Room", HardwareId = "o-storage", ModifiedAt = DateTime.Now }
            };
        }
    }
}
