using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private string jwtKey;

    public AuthController()
    {
        jwtKey = GenerateSecureKey();
    }

    [HttpPost("login")]
    public IActionResult Login(string username, string password)
    {
        if (string.IsNullOrEmpty(username) || string.IsNullOrEmpty(password))
        {
            return BadRequest("Invalid username or password.");
        }

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1),
            signingCredentials: creds);

        return Ok(new
        {
            token = new JwtSecurityTokenHandler().WriteToken(token)
        });
    }

    private static string GenerateSecureKey()
    {
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[32]; // 256 bits
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}
