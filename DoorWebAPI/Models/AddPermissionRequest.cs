using System.ComponentModel.DataAnnotations;

namespace DoorWebAPI.Models
{
    public class AddPermissionRequest
    {
        [MaxLength(25)]
        public string Role { get; set; } = null!;
        public long DoorId { get; set; }
    }
}
