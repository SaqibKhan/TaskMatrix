using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using System.IO;
using System;
using TaskMatrix.Domain.Enums;

namespace TaskMatrix.WebAPI
{
    public class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly string _logFilePath;
        private readonly string _criticalLogFilePath;

        public RequestLoggingMiddleware(RequestDelegate next)
        {
            _next = next;
            _logFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "ApiRequests.log");
            _criticalLogFilePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "CriticalTaskUpdates.log");
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var method = context.Request.Method;
            var endpoint = context.Request.Path;
            var logEntry = $"{DateTime.UtcNow:u} {method} {endpoint}\n";
            await File.AppendAllTextAsync(_logFilePath, logEntry);

            // Check for high priority task creation or update
            if ((endpoint.StartsWithSegments("/AppTask") && (method == "POST" || method == "PUT")))
            {
                context.Request.EnableBuffering();
                context.Request.Body.Position = 0;
                using var reader = new StreamReader(context.Request.Body, leaveOpen: true);
                var body = await reader.ReadToEndAsync();
                context.Request.Body.Position = 0;

                if (body.Contains("\"priority\":") && body.Contains($"\"{TaskPriority.High}\""))
                {
                    var criticalEntry = $"{DateTime.UtcNow:u} CRITICAL {method} {endpoint} BODY: {body}\n";
                    await File.AppendAllTextAsync(_criticalLogFilePath, criticalEntry);
                }
            }

            await _next(context);
        }
    }
}

