using System.ComponentModel.DataAnnotations;

namespace auth_playground.Models;

public class RefreshTokenRequest
{
    [Required]
    public string AccessToken { get; set; }
    
    [Required]
    public string RefreshToken { get; set; }
}