var builder = DistributedApplication.CreateBuilder(args);

var sqlServer = builder.AddSqlServer("sqlserver")
    .WithDataVolume();

var appDatabase = sqlServer.AddDatabase("appdb");

builder.AddProject<Projects.ApiService>("apiservice")
    .WithReference(appDatabase)
    .WaitFor(appDatabase)
    .WithHttpHealthCheck("/health");

builder.Build().Run();
