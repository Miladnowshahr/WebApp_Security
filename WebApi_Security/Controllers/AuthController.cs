using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace WebApi_Security.Controllers;

[Route("[controller]")]
[ApiController]
public class AuthController : ControllerBase
{
    private readonly byte[] secretKey;
    public AuthController(IConfiguration configuration)
    {
        secretKey = Encoding.ASCII.GetBytes(configuration.GetValue<string>("SecretKey"));
    }
    [HttpPost]
    public IActionResult Authenticate([FromBody] Credential credential)
    {
        if (credential.UserName =="milad" && credential.Password=="nematpour")
        {
            var claims = new List<Claim>();


            var expiresAt = DateTime.UtcNow.AddMinutes(10);

            return Ok(new
            {
                access_token = GenerateToken(claims,expiresAt),
                expires_at = expiresAt
            });
        }
        ModelState.AddModelError("Unauthorized", "You are not authorized to access the endpoint");

        return Unauthorized(ModelState);
    }

    private string GenerateToken(IEnumerable<Claim> claims, DateTime expireAt)
    {
        var jwt = new JwtSecurityToken(
            claims: claims,
            notBefore: DateTime.UtcNow,
            expires: expireAt,
            signingCredentials: new SigningCredentials(new SymmetricSecurityKey(secretKey), SecurityAlgorithms.HmacSha256Signature));
            
        return new JwtSecurityTokenHandler().WriteToken(jwt);
    }
}

public class Credential
{
    public string UserName { get; set; }
    public string Password { get; set; }
}