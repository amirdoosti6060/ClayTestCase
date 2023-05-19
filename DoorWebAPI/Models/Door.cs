namespace DoorWebAPI.Models
{
    public class Door
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public string HardwareId { get; set; } = null!;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;

        public ICollection<Permission> Permissions { get; set; }
    }
}
