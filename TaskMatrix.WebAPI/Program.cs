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
