namespace auth_playground.Models;

public class RegisterResponse
{
    public int Id { get; set; }
    public string Email { get; set; }

    public RegisterResponse(int id, string email)
    {
        Id = id;
        Email = email;
    }
}