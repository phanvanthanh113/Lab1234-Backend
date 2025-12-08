namespace WebApplication1.Models;

public class LevelResult
{
    public int quizResultId { get; set; }

    // FK
    public string userId { get; set; } = null!;

    public int levelId { get; set; }

    // Data fields
    public int score { get; set; }
    public DateTime completionDate { get; set; }

    // Navigation
    public GameLevel? gameLevel { get; set; }
}
