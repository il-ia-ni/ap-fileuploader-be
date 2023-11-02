using FileUploaderBackend.DTOs;
using ProLibrary.Models;

namespace FileUploaderBackend.Services
{
    public interface IAuthServicePSK
    {
        bool AuthenticateUser(string username, string password);
        bool UserExists(string username);
        bool CreateUser(UserLoginDto newUser, string role);
    }
}