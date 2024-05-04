using System.Text.Json.Serialization;

namespace auth_playground.Entities;

public class User
{
    public int Id { get; set; }
    public string Username { get; set; } = String.Empty;
    public string Email { get; set; } = String.Empty;
    [JsonIgnore]
    public string PasswordHash { get; set; }
    [JsonIgnore]
    public List<RefreshToken> RefreshTokens { get; set; }
}