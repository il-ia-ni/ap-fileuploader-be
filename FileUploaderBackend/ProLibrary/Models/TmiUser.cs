using System;
using System.Collections.Generic;

namespace ProLibrary.Models
{
    public partial class TmiUser
    {
        public string Username { get; set; } = null!;
        public string? FirstName { get; set; }
        public string? LastName { get; set; }
        public string? LandingPage { get; set; }
        public string RoleName { get; set; } = null!;
        public DateTime LastModifiedAt { get; set; }
        public byte[]? PasswordHash { get; set; }
        public byte[]? PasswordSalt { get; set; }
    }
}
