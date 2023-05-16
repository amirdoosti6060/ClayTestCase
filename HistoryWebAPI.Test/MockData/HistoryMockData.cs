using HistoryWebAPI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HistoryWebAPI.Test.MockData
{
    public static class HistoryMockData
    {
        public static List<History> GetHistories()
        {
            return new List<History>()
            {
                new History() { Id = 1, DoorId = 1, DoorName = "Main Entrance", HardwareId = "o-main", UserId = 1, FullName = "Administrator",  Email = "admin@test.com", Role = "administrator", ActionStatus = "Ok", TimeStamp = DateTime.Now },
                new History() { Id = 2, DoorId = 2, DoorName = "Storage Room", HardwareId = "o-storage", UserId = 1, FullName = "Administrator",  Email = "admin@test.com", Role = "administrator", ActionStatus = "Fail", TimeStamp = DateTime.Now },
                new History() { Id = 3, DoorId = 2, DoorName = "Storage Room", HardwareId = "o-storage", UserId = 2, FullName = "Manager",  Email = "manager@test.com", Role = "manager", ActionStatus = "Ok", TimeStamp = DateTime.Now },
                new History() { Id = 4, DoorId = 3, DoorName = "Convference 1", HardwareId = "o-conf", UserId = 3, FullName = "User",  Email = "user@test.com", Role = "user", ActionStatus = "Timeout", TimeStamp = DateTime.Now },
                new History() { Id = 5, DoorId = 1, DoorName = "Main Entrance", HardwareId = "o-main", UserId = 3, FullName = "User",  Email = "user@test.com", Role = "user", ActionStatus = "Ok", TimeStamp = DateTime.Now },
            };
        }
    }
}
