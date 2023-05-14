namespace DoorWebAPI.Models
{
    public class GeneralResponse
    {
        public object? ErrorCode { get; set; }
        public string? ErrorMessage { get; set; }
        public object? Data { get; set; }
    }
}
