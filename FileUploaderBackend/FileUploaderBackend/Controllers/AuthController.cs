using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FileUploaderBackend.Services;
using FileUploaderBackend.DTOs;

namespace FileUploaderBackend.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IConfiguration _configuration;
        private readonly IAuthServicePSK _authService;

        public AuthController(IConfiguration configuration, IAuthServicePSK authService)
        {
            _configuration = configuration;
            _authService = authService;
        }

        [HttpPost("login")]
        public IActionResult Login([FromBody] UserLoginDto loginRequestData)
        {
            var username = loginRequestData.Username;
            var password = loginRequestData.Password;

            if (_authService.AuthenticateUser(username, password))
            {
                // Authentifizierung erfolgreich
                // Hier können Sie ein JWT-Token erstellen und zurückgeben
                var token = GenerateJwtToken(username);

                return Ok(new { token });
            }

            // Authentifizierung fehlgeschlagen
            return Unauthorized("Invalid username or password");
        }

        private string GenerateJwtToken(string username)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(
                _configuration.GetSection("TokenKeys:DEV").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username)
                }),
                Expires = DateTime.UtcNow.AddHours(3), // Expiry time of the Token
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature),
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


}
