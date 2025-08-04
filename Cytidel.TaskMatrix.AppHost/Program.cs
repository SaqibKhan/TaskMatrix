var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.TaskMatrix_WebAPI>("taskmatrix-webapi");

builder.Build().Run();
