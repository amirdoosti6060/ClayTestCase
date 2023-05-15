namespace HistoryWebAPI.Controllers
{
    public class DoorUnlockInfo
    {
        public long DoorId { get; set; }
        public string DoorName { get; set; } = null!;
        public string HardwareId { get; set; } = null!;
        public long UserId { get; set; }
        public string FullName { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string Role { get; set; } = null!;
        public string ActionStatus { get; set; } = null!;
        public DateTime TimeStamp { get; set; }
    }
}
