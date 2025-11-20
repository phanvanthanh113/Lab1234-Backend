namespace WebApplication1.Models
{
    public class ResponseAPI
    {
        public bool IsSuccess { get; set; }
        public string? Notification { get; set; }
        public object? Data { get; set; }
    }
}
