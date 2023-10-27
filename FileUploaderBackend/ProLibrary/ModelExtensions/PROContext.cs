using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProLibrary.Models
{
    public partial class PROContext
    {
        private static IConfiguration? _configuration;

        private static string? _connectionString;

        public static void Configure(IConfiguration configuration)
        {
            _configuration = configuration;
            _connectionString = _configuration.GetConnectionString("PRO:SqlServerDEV");  // Choose the right DB before scaffolding
        }
    }
}
