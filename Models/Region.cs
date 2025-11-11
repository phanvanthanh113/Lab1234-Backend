namespace WebApplication1.Models;
using System.Text.Json.Serialization;

public class Region
{
    public int regionId { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public Region() { }

    public Region(int id, string name)
    {
        regionId = id;
        Name = name;
    }
    
    [JsonIgnore]
    public ICollection<User>? Users { get; set; }
}