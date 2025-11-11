namespace WebApplication1.Models;
using System.Text.Json.Serialization;

public class Role
{
    public int roleId { get; set; }
    public string Name { get; set; } = string.Empty;
    
    public Role() { }

    public Role(int id, string name)
    {
        roleId = id;
        Name = name;
    }
    
    [JsonIgnore]
    public ICollection<User>? Users { get; set; }
}