var builder = DistributedApplication.CreateBuilder(args);

builder.AddProject<Projects.ApiService>("apiservice").WithHttpHealthCheck("/health");

builder.Build().Run();
