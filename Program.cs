using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;
using System.IdentityModel.Tokens.Jwt;
using Microsoft.AspNetCore.Authorization;
using System;
using System.Security.Cryptography;


var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddAuthorization();

string jwtKey = builder.Configuration["JwtConfig:Token"] ?? "fallback-secure-key-if-none-configured";

builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(options =>
    {
        options.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuerSigningKey = true,
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)),
            ValidateIssuer = false,
            ValidateAudience = false
        };
    });


var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
    app.UseAuthentication();
    app.UseAuthorization();

}

app.UseHttpsRedirection();

app.MapPost("/login", () =>
{
    var claims = new[]
    {
        new Claim(ClaimTypes.Name, "User"), // Replace with the user's username or other identifier
    };

    var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtKey)); // Replace with a secure key
    var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

    var token = new JwtSecurityToken(
        claims: claims,
        expires: DateTime.Now.AddHours(1),
        signingCredentials: creds);

    return new
    {
        token = new JwtSecurityTokenHandler().WriteToken(token)
    };
})
.WithName("Login");

app.MapPost("/calculate-supply", (HttpContext httpContext) =>
{
    // Simulate the token supply calculation
    var totalSupply = 1000000M; // Example total supply, you would fetch this from the BNB Chain
    var nonCirculating = 200000M; // Example non-circulating amount, sum of specified addresses
    var circulatingSupply = totalSupply - nonCirculating;
    var token = httpContext.Request.Headers["Authorization"].ToString().Replace("Bearer ", "");
    Console.WriteLine($"Token: {token}");
    // Create and return a token info object
    var tokenInfo = new
    {
        Name = "BLP Token",
        TotalSupply = totalSupply,
        CirculatingSupply = circulatingSupply
    };

    return tokenInfo;
})
// .RequireAuthorization()
.WithName("CalculateSupply"); ;

app.Run();

partial class Program
{
    static void Main()
    {
        Console.WriteLine(GenerateSecureKey());
    }

    public static string GenerateSecureKey()
    {
        using (var rng = new RNGCryptoServiceProvider())
        {
            var randomBytes = new byte[32]; // 256 bits
            rng.GetBytes(randomBytes);
            return Convert.ToBase64String(randomBytes);
        }
    }
}