var builder = DistributedApplication.CreateBuilder(args);


var mongo = builder.AddMongoDB("mongo")
                   .WithLifetime(ContainerLifetime.Persistent);

var mongodb = mongo.AddDatabase("mongodb");

var apiService = builder.AddProject<Projects.src_ApiService>("apiservice")
    .WaitFor(mongodb)
    .WithReference(mongodb);

builder.AddProject<Projects.src_Web>("webfrontend")
    .WithExternalHttpEndpoints()
    .WithReference(apiService);

builder.Build().Run();
