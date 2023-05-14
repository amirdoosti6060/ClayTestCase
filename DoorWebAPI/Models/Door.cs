namespace DoorWebAPI.Models
{
    public class Door
    {
        public long Id { get; set; }
        public string Name { get; set; } = null!;
        public byte State { get; set; } = 0;
        public DateTime ModifiedAt { get; set; } = DateTime.Now;
    }
}
