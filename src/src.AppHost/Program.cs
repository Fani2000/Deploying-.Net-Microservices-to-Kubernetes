var builder = DistributedApplication.CreateBuilder(args);

var apiService = builder.AddProject<Projects.src_ApiService>("apiservice");

builder.AddProject<Projects.src_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
