namespace auth_playground.Helpers;

public class AppSettings
{
    public string Secret { get; set; }

    // refresh token time to live (in days)
    public int RefreshTokenTTL { get; set; }
}