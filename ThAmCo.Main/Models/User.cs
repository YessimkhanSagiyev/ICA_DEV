namespace ThAmCo.Main.Models;
public class User
{
    public int UserId { get; set; }
    public string? Name { get; set; }
    public required  string  Email { get; set; }
    public required  string PasswordHash { get; set; }
    public string? Address { get; set; }
    public DateTime CreatedAt { get; set; }
}
