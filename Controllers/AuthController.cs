using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;

[Route("api/[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    [HttpPost("login")]
    public IActionResult Login()
    {
        // In a real application, you should validate the username and password.
        // For simplicity, we are skipping that step.

        var claims = new[]
        {
            new Claim(ClaimTypes.Name, "User"),
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes("YourVerySecretKey"));
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
}
