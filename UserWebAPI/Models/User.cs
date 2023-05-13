namespace UserWebAPI.Models
{
    public class User
    {
        public long Id { get; set; }
        public string Email { get; set; } = null!;
        public string Password { get; set; } = null!;
        public string Role { get; set; } = null!;
        public byte State { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.Now;
    }
}
