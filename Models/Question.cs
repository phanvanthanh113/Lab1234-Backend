namespace WebApplication1.Models;

public class Question
{
    public int questionId { get; set; }
    public string contentQuestion { get; set; } = string.Empty;
    public string answer { get; set; } = string.Empty;
    public string option1 { get; set; } = string.Empty;
    public string option2 { get; set; } = string.Empty;
    public string option3 { get; set; } = string.Empty;
    public string option4 { get; set; } = string.Empty;

    // FK
    public int levelId { get; set; }
    public GameLevel? gameLevel { get; set; }
}
