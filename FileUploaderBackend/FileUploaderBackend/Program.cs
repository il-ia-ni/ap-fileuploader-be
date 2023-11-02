using FileUploaderBackend.Services;
using Microsoft.EntityFrameworkCore;
using ProLibrary.Models;
using Newtonsoft.Json;

var builder = WebApplication.CreateBuilder(args);

// Add services to the container.

builder.Services.AddControllers()
    .AddNewtonsoftJson();

// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

builder.Services.AddDbContext<PROContext>(options => 
    options.UseSqlServer(builder.Configuration.GetConnectionString("PRO:SqlServerDEV")));

builder.Services.AddScoped<IAuthServicePSK, AuthServicePSK>();
builder.Services.AddTransient<IProRepository, ProRepository>();
builder.Services.AddTransient<IExcelReaderService, ExcelReaderService>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();

app.UseAuthorization();

app.MapControllers();

app.Run();
