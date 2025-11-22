using src.ApiService.Endpoints;
using src.ApiService.Services;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddProblemDetails();
builder.Services.AddCors(options =>
{
    options.AddDefaultPolicy(policy =>
    {
        policy.AllowAnyOrigin()
              .AllowAnyMethod()
              .AllowAnyHeader();
    });
});

// Add MongoDB
builder.AddMongoDBClient(connectionName: "mongodb");
builder.Services.AddSingleton<ProductService>();

// Add Swagger/OpenAPI
builder.Services.AddOpenApi();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{

    app.MapOpenApi();
    app.UseSwaggerUI(options =>
    {
        options.SwaggerEndpoint("/openapi/v1.json", "v1");
    });
}

app.UseExceptionHandler();
app.UseCors();

// Redirect root to Swagger in development
if (app.Environment.IsDevelopment())
{
    app.MapGet("/", () => Results.Redirect("/swagger"));
}

// Seed MongoDB with initial data
using (var scope = app.Services.CreateScope())
{
    var productService = scope.ServiceProvider.GetRequiredService<ProductService>();
    var seedProducts = ProductSeedService.GetSeedProducts();
    await productService.SeedProductsAsync(seedProducts);
}

// Map endpoints
app.MapProductEndpoints();
app.MapWeatherForecastEndpoints();
app.MapDefaultEndpoints();

// app.MapSwagger();

app.Run();
