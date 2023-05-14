namespace DoorWebAPI.Models
{
    public class Permission
    {
        public long Id { get; set; }
        public string Role { get; set; } = null!;
        public long DoorId { get; set; }
    }
}
