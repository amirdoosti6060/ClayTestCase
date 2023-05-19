using System.ComponentModel.DataAnnotations;

namespace DoorWebAPI.Models
{
    public class GetPermissionRequest
    {
        public long? doorId { get; set; }

        [MaxLength(25)]
        public string? role { get; set; }
    }
}
