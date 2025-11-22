using src.ApiService.Models;
using src.ApiService.Services;

namespace src.ApiService.Endpoints;

public static class ProductEndpoints
{
    public static void MapProductEndpoints(this WebApplication app)
    {
        var group = app.MapGroup("/api/products")
            .WithTags("Products");

        group.MapGet("/", GetAllProducts)
            .WithName("GetProducts")
            .Produces<List<Product>>(StatusCodes.Status200OK)
            .WithSummary("Get all products")
            .WithDescription("Retrieves a list of all available products");

        group.MapGet("/{id}", GetProductById)
            .WithName("GetProductById")
            .Produces<Product>(StatusCodes.Status200OK)
            .Produces(StatusCodes.Status404NotFound)
            .WithSummary("Get product by ID")
            .WithDescription("Retrieves a specific product by its unique identifier");

        group.MapGet("/category/{category}", GetProductsByCategory)
            .WithName("GetProductsByCategory")
            .Produces<List<Product>>(StatusCodes.Status200OK)
            .WithSummary("Get products by category")
            .WithDescription("Retrieves all products in a specific category");
    }

    private static async Task<IResult> GetAllProducts(ProductService productService)
    {
        var products = await productService.GetAllProductsAsync();
        return Results.Ok(products);
    }

    private static async Task<IResult> GetProductById(string id, ProductService productService)
    {
        var product = await productService.GetProductByIdAsync(id);
        if (product == null)
        {
            return Results.NotFound();
        }
        return Results.Ok(product);
    }

    private static async Task<IResult> GetProductsByCategory(string category, ProductService productService)
    {
        var products = await productService.GetProductsByCategoryAsync(category);
        return Results.Ok(products);
    }
}

