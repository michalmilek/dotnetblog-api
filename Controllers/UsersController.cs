using auth_playground.Models;
using auth_playground.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace auth_playground.Controllers;


[Authorize]
[ApiController]
[Route("[controller]")]
public class UsersController : ControllerBase
{
    private IUserService _userService;
    
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }
    
    [AllowAnonymous]
    [HttpPost("register")]
    public IActionResult Register(RegisterRequest model)
    {
        _userService.Register(model);
        return Ok(new { message = "Registration successful" });
    }
    
    [AllowAnonymous]
    [HttpPost("login")]
    public IActionResult Login(AuthenticateRequest model)
    {
        var response = _userService.Authenticate(model, ipAddress());
        return Ok(response);
    }
    
    [AllowAnonymous]
    [HttpPost("refresh-token")]
    public IActionResult RefreshToken(RefreshTokenRequest model)
    {
        var response = _userService.RefreshToken(model);
        return Ok(response);
    }
    
    [HttpPost("revoke-token")]
    public IActionResult RevokeToken(RefreshTokenResponse model)
    {
        _userService.InvalidateRefreshToken(model.RefreshToken);
        return Ok(new { message = "Token revoked" });
    }


    private string ipAddress()
    {
        // get source ip address for the current request
        if (Request.Headers.ContainsKey("X-Forwarded-For"))
            return Request.Headers["X-Forwarded-For"];

        return HttpContext.Connection.RemoteIpAddress.MapToIPv4().ToString();
    }
}