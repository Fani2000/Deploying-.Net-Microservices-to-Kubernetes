using src.ApiService.Models;

namespace src.ApiService.Endpoints;

public static class WeatherForecastEndpoints
{
    private static readonly string[] Summaries = new[]
    {
        "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
    };

    public static void MapWeatherForecastEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/weatherforecast")
            .WithTags("Weather");

        group.MapGet("/", GetWeatherForecast)
            .WithName("GetWeatherForecast")
            .Produces<WeatherForecast[]>(StatusCodes.Status200OK)
            .WithSummary("Get weather forecast")
            .WithDescription("Retrieves a 5-day weather forecast");
    }

    private static IResult GetWeatherForecast()
    {
        var forecast = Enumerable.Range(1, 5).Select(index =>
            new WeatherForecast
            (
                DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                Random.Shared.Next(-20, 55),
                Summaries[Random.Shared.Next(Summaries.Length)]
            ))
            .ToArray();
        return Results.Ok(forecast);
    }
}

