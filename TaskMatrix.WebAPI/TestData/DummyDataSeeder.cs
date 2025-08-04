using Microsoft.AspNetCore.Mvc;
using System.Text.Json;
using TaskMatrix.Application.DTOs;
using TaskMatrix.Application.Interfaces;
using TaskMatrix.Domain.Entities;
using TaskMatrix.Infrastructure.Data;

namespace TaskMatrix.WebAPI.TestData
{
    public static class DummyDataSeeder
    {
        public static async Task SeedAsync(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var _iAppTaskService = scope.ServiceProvider.GetRequiredService<IAppTaskService>();
            string filePath = "TestData/random_tasks_5000.json";
            using FileStream openStream = System.IO.File.OpenRead(filePath);
            var options = new JsonSerializerOptions { PropertyNameCaseInsensitive = true };
            List<CreateAppTaskDto> tasks = await JsonSerializer.DeserializeAsync<List<CreateAppTaskDto>>(openStream, options);
            if (tasks != null && tasks.Any())
            {
                foreach (var task in tasks)
                {
                    await _iAppTaskService.CreateAsync(task);
                }
            }
        }
    }

}
