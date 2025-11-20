namespace WebApplication1.Models;

public class LevelResult
{
    public int quizResultId { get; set; }

    // FK
    public int userId { get; set; }
    public int levelId { get; set; }

    // Data fields
    public int score { get; set; }
    public DateTime completionDate { get; set; }

    // Navigation
    public User? user { get; set; }
    public GameLevel? gameLevel { get; set; }
}
