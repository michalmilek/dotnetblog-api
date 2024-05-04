using auth_playground.Entities;

namespace auth_playground.Models;

public class AuthenticateResponse
{
    public int Id { get; set; }
    public string Email { get; set; }
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    
    public AuthenticateResponse(User user, string accessToken, string refreshToken)
    {
        Id = user.Id;
        Email = user.Email;
        AccessToken = accessToken;
        RefreshToken = refreshToken;
    }
}