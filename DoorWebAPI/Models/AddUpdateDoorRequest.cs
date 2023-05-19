using System.ComponentModel.DataAnnotations;

namespace DoorWebAPI.Models
{
    public class AddUpdateDoorRequest
    {
        [MaxLength(30)]
        public string Name { get; set; } = null!;

        [MaxLength(30)]
        public string HardwareId { get; set; } = null!;
    }
}
