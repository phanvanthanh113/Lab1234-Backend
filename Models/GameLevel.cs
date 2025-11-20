namespace WebApplication1.Models;

public class GameLevel
{
    public int levelId { get; set; }
    public string title { get; set; } = string.Empty;
    public string description { get; set; } = string.Empty;

    // Navigation
    public ICollection<Question>? questions { get; set; }
}
