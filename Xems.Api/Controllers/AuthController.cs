using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Xems.Api.Models;

namespace Xems.Api.Controllers;

[ApiController]
[Route("auth")]
public class AuthController : ControllerBase
{
	private readonly IConfiguration _configuration;

	public AuthController(IConfiguration configuration)
	{
		_configuration = configuration;
	}

	[HttpPost("token")]
	public IActionResult CreateToken(LoginRequestDto request)
	{
		var role = GetRole(request.Username, request.Password);

		if (role is null)
			return Unauthorized("Invalid username or password.");

		var token = GenerateToken(request.Username, role);

		return Ok(new
		{
			accessToken = token,
			tokenType = "Bearer"
		});
	}

	private static string? GetRole(string username, string password)
	{
		if (username == "admin" && password == "admin123")
			return "Admin";

		if (username == "operator" && password == "operator123")
			return "Operator";

		if (username == "guest" && password == "guest123")
			return "Guest";

		return null;
	}

	private string GenerateToken(string username, string role)
	{
		var jwtSection = _configuration.GetSection("Jwt");

		var claims = new List<Claim>
				{
						new(ClaimTypes.Name, username),
						new(ClaimTypes.Role, role)
				};

		var key = new SymmetricSecurityKey(
				Encoding.UTF8.GetBytes(jwtSection["Secret"]!));

		var credentials = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

		var expiresMinutes = int.Parse(jwtSection["ExpiresMinutes"]!);

		var token = new JwtSecurityToken(
				issuer: jwtSection["Issuer"],
				audience: jwtSection["Audience"],
				claims: claims,
				expires: DateTime.UtcNow.AddMinutes(expiresMinutes),
				signingCredentials: credentials);

		return new JwtSecurityTokenHandler().WriteToken(token);
	}
}