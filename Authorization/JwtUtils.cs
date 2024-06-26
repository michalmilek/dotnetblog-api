﻿using auth_playground.Entities;
using auth_playground.Helpers;
using Microsoft.Extensions.Options;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using Microsoft.Extensions.Primitives;
using Microsoft.IdentityModel.Tokens;

namespace auth_playground.Authorization;

public interface IJwtUtils
{
    string GenerateJwtToken(User user);
    public int? ValidateToken(string token);
    RefreshToken GenerateRefreshToken(string ipAddress);
    string getRefreshToken();

    public int? GetUserIdFromToken();
}

public class JwtUtils : IJwtUtils
{
    private DataContext _context;
    private readonly AppSettings _appSettings;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtUtils(
        DataContext context,
        IOptions<AppSettings> appSettings,
        IHttpContextAccessor httpContextAccessor)
    {
        _context = context;
        _appSettings = appSettings.Value;
        _httpContextAccessor = httpContextAccessor;
    }

    public string GenerateJwtToken(User user)
    {
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = System.Text.Encoding.ASCII.GetBytes(_appSettings.Secret);
        var tokenDescriptor = new SecurityTokenDescriptor
        {
            Subject = new ClaimsIdentity(new[] { new Claim("id", user.Id.ToString()) }),
            Expires = DateTime.UtcNow.AddMinutes(15),
            SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
        };

        var token = tokenHandler.CreateToken(tokenDescriptor);
        return tokenHandler.WriteToken(token);
    }
    
    public int? ValidateToken(string token)
    {
        if (string.IsNullOrEmpty(token))
            return null;
        
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = System.Text.Encoding.UTF8.GetBytes(_appSettings.Secret);
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out SecurityToken validatedToken);

            var jwtToken = (JwtSecurityToken)validatedToken;
            var userId = int.Parse(jwtToken.Claims.First(x => x.Type == "id").Value);

            return userId;
        }
        catch
        {
            return null;
        }
    }
    
    public RefreshToken GenerateRefreshToken(string ipAddress)
    {
        try
        {
            var rfrToken = new RefreshToken
            {
                Token = getRefreshToken(),  // Ensure getRefreshToken() is not failing
                Expires = DateTime.UtcNow.AddDays(_appSettings.RefreshTokenTTL),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            Console.WriteLine(rfrToken);
            return rfrToken;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"An error occurred: {ex.Message}");
            throw;  // Or handle the error appropriately
        }
    }

    public string getRefreshToken()
    {
        var token = Convert.ToBase64String(RandomNumberGenerator.GetBytes(64));
        var isUniqueToken = !_context.Users.Any(user => user.RefreshTokens.Any(T => T.Token == token));
        
        if (isUniqueToken)
            return token;

        return token;
    }
    
    public int? GetUserIdFromToken()
    {
        var accessTokenValue = _httpContextAccessor.HttpContext.Request.Headers["Authorization"];
        if (StringValues.IsNullOrEmpty(accessTokenValue))
        {
            return null;
        }

        var accessToken = accessTokenValue.ToString();
        if (string.IsNullOrEmpty(accessToken) || !accessToken.StartsWith("Bearer ") || accessToken.Length <= "Bearer ".Length)
        {
            return null;
        }

        // Extract the token from the Authorization header
        var token = accessToken.Substring("Bearer ".Length).Trim();

        // Validate and decode the token
        var tokenHandler = new JwtSecurityTokenHandler();
        var key = System.Text.Encoding.UTF8.GetBytes(_appSettings.Secret);
        SecurityToken validatedToken;
        try
        {
            tokenHandler.ValidateToken(token, new TokenValidationParameters
            {
                ValidateIssuerSigningKey = true,
                IssuerSigningKey = new SymmetricSecurityKey(key),
                ValidateIssuer = false,
                ValidateAudience = false,
                ClockSkew = TimeSpan.Zero
            }, out validatedToken);
        }
        catch
        {
            return null;
        }

        // Extract the user ID from the token
        var jwtToken = (JwtSecurityToken)validatedToken;
        var idClaim = jwtToken.Claims.First(x => x.Type == "id");
        if (idClaim != null && int.TryParse(idClaim.Value, out var userId))
        {
            return userId;
        }

        // Return null if the user ID cannot be retrieved
        return null;
    }
    
}