using src.Web;
using src.Web.Components;

var builder = WebApplication.CreateBuilder(args);

// Add service defaults & Aspire components.
builder.AddServiceDefaults();

// Add services to the container.
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

builder.Services.AddOutputCache();

// Add HttpClient for ProductApiClient
// Use environment variable if set (Docker Compose), otherwise use Aspire service discovery
var apiServiceUrl = builder.Configuration["Services:apiservice"] ?? "https+http://apiservice";
builder.Services.AddHttpClient<ProductApiClient>(client =>
{
    client.BaseAddress = new Uri(apiServiceUrl);
});

// Add HttpClient for WeatherApiClient
builder.Services.AddHttpClient<WeatherApiClient>(client =>
{
    client.BaseAddress = new Uri(apiServiceUrl);
});

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();

app.UseStaticFiles();
app.UseAntiforgery();

app.UseOutputCache();

app.MapRazorComponents<App>()
.AddInteractiveServerRenderMode();

app.MapDefaultEndpoints();

app.Run();
