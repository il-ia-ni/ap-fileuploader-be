using FileUploaderBackend.Services;
using Microsoft.EntityFrameworkCore;
using ProLibrary.Models;
using Newtonsoft.Json;
using Microsoft.Net.Http.Headers;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Security.Claims;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddCors(options =>
{
    options.AddPolicy(name: "devPolicy",
        cfg =>
        {
            cfg.WithOrigins("http://localhost:4200")
            .WithMethods("GET", "POST", "PUT", "OPTIONS")
            .WithHeaders(HeaderNames.ContentType)
            .AllowCredentials();
        });
});

builder.Services.AddAuthentication(options =>
{
    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
}).AddJwtBearer(options =>
{
    options.TokenValidationParameters = new TokenValidationParameters
    {
        ValidateIssuer = true,
        ValidateAudience = true,
        ValidateLifetime = true,
        ValidateIssuerSigningKey = true,
        ValidIssuer = builder.Configuration["Jwt:Issuer"],
        ValidAudience = builder.Configuration["Jwt:Audience"],
        IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["TokenKeys:DEV"]))
    };
});

builder.Services.AddAuthorization(options =>
{
    options.AddPolicy("AdminOnly", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "admin");
    });
    options.AddPolicy("AdminOrUser", policy =>
    {
        policy.RequireClaim(ClaimTypes.Role, "admin", "user");
    });
});

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson(options =>
    {
        options.SerializerSettings.Formatting = Formatting.Indented;
    });

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PROContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("PRO:SqlServerDEV")));

builder.Services.AddScoped<IAuthServicePSK, AuthServicePSK>();
builder.Services.AddTransient<IProRepository, ProRepository>();
builder.Services.AddTransient<IExcelReaderService, ExcelReaderService>();

var app = builder.Build();

app.UseCors("devPolicy");

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthentication();
app.UseAuthorization();

app.MapControllers();

app.Run();
