namespace HistoryWebAPI.Models
{
    public class HistoryRequest
    {
        public long? doorId { get; set; }
        public long? userId { get; set; }
        public int? year { get; set; }
        public int? month { get; set; }
        public int? day { get; set; }
        public string? role { get; set; }
        public int? top { get; set; }
    }
}
