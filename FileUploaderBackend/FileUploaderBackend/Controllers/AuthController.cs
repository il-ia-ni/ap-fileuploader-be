using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using FileUploaderBackend.Services;
using FileUploaderBackend.DTOs;
using FileUploaderWebAPI.DTOs;
using Microsoft.EntityFrameworkCore;

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
        [ProducesResponseType(StatusCodes.Status200OK, Type = typeof(AuthenticationResponseDto))]
        [ProducesResponseType(StatusCodes.Status400BadRequest, Type = typeof(AuthenticationResponseDto))]
        [ProducesResponseType(StatusCodes.Status500InternalServerError, Type = typeof(AuthenticationResponseDto))]
        public IActionResult Login([FromBody] UserLoginDto loginRequestData)
        {
            try
            {
                if (_isValidLoginData(loginRequestData))
                {
                    var username = loginRequestData.Username;
                    var password = loginRequestData.Password;

                    if (_authService.AuthenticateUser(username, password))
                    {
                        var tmiUser = _authService.GetUser(username);
                        var userRole = tmiUser != null ? tmiUser.RoleName : null;
                        // Authentification succeeded
                        var token = _generateJwtToken(username, userRole);

                        return Ok(new AuthenticationResponseDto()
                        {
                            Token = token
                        });
                    }

                    // Authentification failed
                    return Unauthorized(new AuthenticationResponseDto()
                    {
                        Error = "Invalid username or password"
                    });
                }

                return UnprocessableEntity(new AuthenticationResponseDto()
                {
                    Error = "Missing user credentials username / password"
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new AuthenticationResponseDto()
                {
                    Error = $"Internal server error: {ex.Message}"
                });
            }
        }

        private bool _isValidLoginData(UserLoginDto userData)
        {
            return !string.IsNullOrEmpty(userData.Username) && !string.IsNullOrEmpty(userData.Password);
        }

        private string _generateJwtToken(string username, string? role = "guest")
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(
                _configuration.GetSection("TokenKeys:DEV").Value);
            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(new[]
                {
                    new Claim(ClaimTypes.Name, username),
                    new Claim(ClaimTypes.Role, role)
                }),
                Expires = DateTime.UtcNow.AddHours(3), // Expiry time of the Token
                SigningCredentials = new SigningCredentials(
                    new SymmetricSecurityKey(key), 
                    SecurityAlgorithms.HmacSha256Signature),  // Key of HMAC-SHA256-Algorithm min 256 bits (32 Bytes)
                Issuer = _configuration.GetSection("Jwt:Issuer").Value,
                Audience = _configuration.GetSection("Jwt:Audience").Value
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }
    }


}
