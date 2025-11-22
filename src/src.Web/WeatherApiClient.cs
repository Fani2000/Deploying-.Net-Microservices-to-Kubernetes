using src.Web.Models;

namespace src.Web;

public class WeatherApiClient
{
    private readonly HttpClient _httpClient;

    public WeatherApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<WeatherForecast[]> GetWeatherForecastAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<WeatherForecast[]>("/weatherforecast", cancellationToken);
        return response ?? Array.Empty<WeatherForecast>();
    }
}
