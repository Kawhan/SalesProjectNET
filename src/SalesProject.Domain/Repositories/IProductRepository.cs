using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Domain.Repositories;

/// <summary>
/// Repository interface for product entity operations
/// </summary>
public interface IProductRepository
{
    /// <summary>
    /// Creates a new product in the repository
    /// </summary>
    /// <param name="product">The product to create</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created product</returns>
    Task<Product> CreateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Update an existing product in the repository.
    /// </summary>
    /// <param name="product">The product to update</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The updated product</returns>
    Task<Product> UpdateAsync(Product product, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a product by its unique identifier
    /// </summary>
    /// <param name="id">The unique identifier of the product</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The product if found, null otherwise</returns>
    Task<Product?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a product from the repository
    /// </summary>
    /// <param name="id">The unique identifier of the product to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the product was deleted, false if not found</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a product by its name
    /// </summary>
    /// <param name="name">The name of the product</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The product if found, null otherwise</returns>
    Task<Product?> GetByNameAsync(string name, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves products by their unique identifiers.
    /// </summary>
    /// <param name="ids">The unique identifiers of the products.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of products found.</returns>
    Task<List<Product>> GetByIdsAsync(IEnumerable<Guid> ids, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves products with pagination and filters.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="name">The product name filter.</param>
    /// <param name="minPrice">The minimum product price filter.</param>
    /// <param name="maxPrice">The maximum product price filter.</param>
    /// <param name="status">The product status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of products found and the total count.</returns>
    Task<(List<Product> Products, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? name = null,
        decimal? minPrice = null,
        decimal? maxPrice = null,
        ProductStatus? status = null,
        CancellationToken cancellationToken = default);

}
