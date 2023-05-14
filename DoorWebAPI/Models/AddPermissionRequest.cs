namespace DoorWebAPI.Models
{
    public class AddPermissionRequest
    {
        public string Role { get; set; } = null!;
        public long DoorId { get; set; }
    }
}
