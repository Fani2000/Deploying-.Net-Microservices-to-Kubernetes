using src.Web.Models;

namespace src.Web;

public class ProductApiClient
{
    private readonly HttpClient _httpClient;

    public ProductApiClient(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<List<Product>> GetProductsAsync(CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<List<Product>>("/api/products", cancellationToken);
        return response ?? new List<Product>();
    }

    public async Task<Product?> GetProductByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        return await _httpClient.GetFromJsonAsync<Product>($"/api/products/{id}", cancellationToken);
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(string category, CancellationToken cancellationToken = default)
    {
        var response = await _httpClient.GetFromJsonAsync<List<Product>>($"/api/products/category/{category}", cancellationToken);
        return response ?? new List<Product>();
    }
}

