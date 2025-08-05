using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using Microsoft.EntityFrameworkCore;
using System.Text.Json;
using TaskMatrix.Application.DTOs;
using TaskMatrix.Application.Interfaces;
using TaskMatrix.Application.Services;
using TaskMatrix.Domain.Interfaces;
using TaskMatrix.Infrastructure.Data;
using TaskMatrix.Infrastructure.Repositories;
using TaskMatrix.WebAPI;
using TaskMatrix.WebAPI.TestData;

namespace TaskMatrix.WebAPI;

public class Program
{
    public static async Task Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);
        builder.AddServiceDefaults();

        // Add services to the container.
        builder.Services.AddCors(options =>
        {
            options.AddPolicy("AllowFrontend",
                builder =>
                {
                    builder.WithOrigins("http://localhost:54396") // frontend URL
                        .AllowAnyMethod()
                        .AllowAnyHeader();
                });
        });
        builder.Services.AddControllers();
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();

        builder.Services.AddDbContext<ApplicationDbContext>(options =>
            options.UseInMemoryDatabase("CyticleDb"));

        builder.Services.AddScoped<IAppTaskRepository, AppTaskRepository>();
        builder.Services.AddScoped<IAppTaskService, AppTaskService>();
       

        // Add JWT authentication
        builder.Services.AddAuthentication(options =>
        {
            options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
            options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
        })
        .AddJwtBearer(options =>
        {
            var jwtSettings = builder.Configuration.GetSection("Jwt");
            options.TokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = true,
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtSettings["Issuer"],
                ValidAudience = jwtSettings["Audience"],
                IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSettings["Key"]))
            };
        });

        builder.Services.AddAuthorization();

        var app = builder.Build();
        // Use CORS
        app.UseCors("AllowFrontend");
        
        using (var scope = app.Services.CreateScope())
        {
            var services = scope.ServiceProvider;
            await DummyDataSeeder.SeedAsync(services);
        }
        // logging middleware
        app.UseMiddleware<RequestLoggingMiddleware>();
        app.UseAuthentication();
        app.UseAuthorization();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }
        app.MapDefaultEndpoints();
        // Configure the HTTP request pipeline.

        app.UseHttpsRedirection();
        app.UseAuthorization();
        app.MapControllers();
        app.Run();
    }


   
}
