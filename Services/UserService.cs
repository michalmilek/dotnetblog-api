﻿using auth_playground.Authorization;
using auth_playground.Entities;
using auth_playground.Helpers;
using auth_playground.Models;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Microsoft.Extensions.Options;
using BCryptNET = BCrypt.Net.BCrypt;

namespace auth_playground.Services;

public interface IUserService
{
    RegisterResponse Register(RegisterRequest model);
    AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
    RefreshTokenResponse RefreshToken(RefreshTokenRequest model);
    void InvalidateRefreshToken(string token);
    
    void RemoveAllRefreshTokens(int userId);
    
    void RemoveOldRefreshTokens(int userId);
    
    IEnumerable<User> GetAll();
}

public class UserService : IUserService
{
    private DataContext _context;
    private IJwtUtils _jwtUtils;
    private readonly AppSettings _appSettings;
    
    public UserService(
        DataContext context,
        IJwtUtils jwtUtils,
        IOptions<AppSettings> appSettings
        )
    {
        _context = context;
        _jwtUtils = jwtUtils;
        _appSettings = appSettings.Value;
    }
    
    public RegisterResponse Register(RegisterRequest model)
    {
        if (_context.Users.Any(user => user.Email == model.Email))
            throw new AppException($"Email {model.Email} is already registered");
        
        var user = new User
        {
            Email = model.Email,
            PasswordHash = BCryptNET.HashPassword(model.Password)
        };
        
        _context.Users.Add(user);
        _context.SaveChanges();
        
        return new RegisterResponse(user.Id, user.Email);
    }
    
    public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
    {
        var user = _context.Users.SingleOrDefault(user => user.Email == model.Email);
        if (user == null || !BCryptNET.Verify(model.Password, user.PasswordHash))
            throw new AppException("Username or password is incorrect");

        var jwtToken = _jwtUtils.GenerateJwtToken(user);
        var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
        user.RefreshTokens.Add(refreshToken);
        
        _context.Users.Update(user);
        _context.SaveChanges();
        
        return new AuthenticateResponse(user, jwtToken, refreshToken.Token);
    }
    
    public RefreshTokenResponse RefreshToken(RefreshTokenRequest model)
    {
        var user = _context.Users.SingleOrDefault(user => user.RefreshTokens.Any(t => t.Token == model.RefreshToken));
        if (user == null)
            throw new AppException("Invalid token");

        var refreshToken = user.RefreshTokens.Single(x => x.Token == model.RefreshToken);
        if (!refreshToken.IsActive)
            throw new AppException("Invalid token");
        
        _context.Users.Update(user);
        _context.SaveChanges();
        
        var jwtToken = _jwtUtils.GenerateJwtToken(user);
        return new RefreshTokenResponse(jwtToken, refreshToken.Token);
    }
    
    public void InvalidateRefreshToken(string token)
    {
        var user = _context.Users.SingleOrDefault(user => user.RefreshTokens.Any(t => t.Token == token));
        if (user == null)
            throw new AppException("Invalid token");

        var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
        refreshToken.Revoked = DateTime.UtcNow;
        refreshToken.RevokedByIp = null;
        refreshToken.ReplacedByToken = null;
        refreshToken.ReasonRevoked = "Token revoked";
        
        _context.Users.Update(user);
        _context.SaveChanges();
    }
    
    public void RemoveAllRefreshTokens(int userId)
    {
        var user = _context.Users.Find(userId);
        user?.RefreshTokens.Clear();

        if (user == null) return;
        _context.Users.Update(user);
        _context.SaveChanges();
    }
    
    public void RemoveOldRefreshTokens(int userId)
    {
        var user = _context.Users.Find(userId);
        user.RefreshTokens.RemoveAll(x => x.Expires <= DateTime.UtcNow);
        _context.Users.Update(user);
        _context.SaveChanges();
    }
    
    public IEnumerable<User> GetAll()
    {
        return _context.Users;
    }
}