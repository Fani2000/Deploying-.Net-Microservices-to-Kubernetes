using MongoDB.Driver;
using src.ApiService.Models;

namespace src.ApiService.Services;

public class ProductService
{
    private readonly IMongoCollection<Product> _products;

    public ProductService(IMongoClient mongoClient, IConfiguration configuration)
    {
        // Get database name from configuration or use default
        var databaseName = configuration["MongoDB:DatabaseName"] ?? "ProductDB";
        var database = mongoClient.GetDatabase(databaseName);
        _products = database.GetCollection<Product>("Products");
    }

    public async Task<List<Product>> GetAllProductsAsync()
    {
        return await _products.Find(_ => true).ToListAsync();
    }

    public async Task<Product?> GetProductByIdAsync(string id)
    {
        return await _products.Find(p => p.Id == id).FirstOrDefaultAsync();
    }

    public async Task<List<Product>> GetProductsByCategoryAsync(string category)
    {
        return await _products.Find(p => p.Category != null && p.Category.Equals(category, StringComparison.OrdinalIgnoreCase)).ToListAsync();
    }

    public async Task SeedProductsAsync(List<Product> products)
    {
        // Check if products already exist
        var existingCount = await _products.CountDocumentsAsync(_ => true);
        if (existingCount > 0)
        {
            return; // Already seeded
        }

        // Insert seed data
        await _products.InsertManyAsync(products);
    }
}

