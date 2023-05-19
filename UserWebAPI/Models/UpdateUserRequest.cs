using System.ComponentModel.DataAnnotations;

namespace UserWebAPI.Models
{
    public class UpdateUserRequest
    {
        [MaxLength(50)]
        public string Email { get; set; } = null!;

        [MaxLength(50)]
        public string Password { get; set; } = null!;

        [MaxLength(40)]
        public string FullName { get; set; } = null!;

        [MaxLength(25)]
        public string Role { get; set; } = null!;
    }
}
