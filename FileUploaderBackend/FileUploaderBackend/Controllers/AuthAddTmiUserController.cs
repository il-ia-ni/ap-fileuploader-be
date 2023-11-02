using FileUploaderBackend.DTOs;
using FileUploaderBackend.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using ProLibrary.Models;

namespace FileUploaderBackend.Controllers
{
    //[Authorize(Roles = "admin")]
    [Route("api/auth/adduser")]
    [ApiController]
    public class AuthAddTmiUserController : Controller
    {
        private readonly IAuthServicePSK _authService;
        public AuthAddTmiUserController(IAuthServicePSK authService)
        {
            _authService = authService;
        }

        [HttpPost]
        public IActionResult AddUser([FromBody] UserLoginDto newUser, string role = "")
        {
            if (string.IsNullOrEmpty(newUser.Username) || string.IsNullOrEmpty(newUser.Password))
            {
                return BadRequest("Username and password are required.");
            }

            if (_authService.UserExists(newUser.Username))
            {
                return BadRequest("User with the same username already exists.");
            }

            bool isUserCreated = _authService.CreateUser(newUser, role);

            if (isUserCreated)
            {
                return Ok("User registration successful.");
            }
            else
            {
                return StatusCode(500, "Failed to create user. Please try again.");
            }
        }
    }
}
