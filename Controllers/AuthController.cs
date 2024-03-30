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
    private static string _generatedToken;

    [HttpPost("login")]
    public IActionResult Login([FromBody] LoginModel model)
    {
        if (model == null || string.IsNullOrEmpty(model.Username) || string.IsNullOrEmpty(model.Password))
        {
            return BadRequest("Invalid username or password.");
        }

        // Generate token only if it's not already generated
        if (string.IsNullOrEmpty(_generatedToken))
        {
            _generatedToken = GenerateToken(model.Username);
        }

        return Ok(new
        {
            token = _generatedToken
        });
    }

    private static string GenerateToken(string username)
    {
        var claims = new[]
        {
            new Claim(ClaimTypes.Name, username),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(GenerateSecureKey()));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var token = new JwtSecurityToken(
            claims: claims,
            expires: DateTime.Now.AddHours(1), // Adjust token lifetime as needed
            signingCredentials: creds);

        return new JwtSecurityTokenHandler().WriteToken(token);
    }

    private static string GenerateSecureKey()
    {
        using (var rng = new System.Security.Cryptography.RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[32]; // 256 bits
            rng.GetBytes(randomBytes);

            // Convert bytes to Base64 string
            return Convert.ToBase64String(randomBytes);
        }
    }

    public static string GetGeneratedToken()
    {
        if (_generatedToken == null)
        {
            _generatedToken = GenerateToken("default_username"); // Provide a default username or obtain it from configuration
        }
        return _generatedToken;
    }
}
