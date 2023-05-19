using System.ComponentModel.DataAnnotations;

namespace HistoryWebAPI.Models
{
    public class AddHistoryRequest
    {
        public long DoorId { get; set; }

        [MaxLength(30)]
        public string DoorName { get; set; } = null!;

        [MaxLength(30)]
        public string HardwareId { get; set; } = null!;

        public long UserId { get; set; }

        [MaxLength(50)]
        public string FullName { get; set; } = null!;

        [MaxLength(50)]
        public string Email { get; set; } = null!;

        [MaxLength(25)]
        public string Role { get; set; } = null!;

        [MaxLength(50)]
        public string ActionStatus { get; set; } = null!;

        public DateTime TimeStamp { get; set; }
    }
}
