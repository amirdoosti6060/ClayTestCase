using System.ComponentModel.DataAnnotations;

namespace UserWebAPI.Models
{
    public class LoginRequest
    {
        [MaxLength(50)]
        public string? Email { get; set; }

        [MaxLength(50)]
        public string? Password { get; set; }
    }
}
