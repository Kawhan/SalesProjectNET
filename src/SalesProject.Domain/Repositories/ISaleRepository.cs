using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Domain.Repositories;

/// <summary>
/// Defines repository operations for managing sales.
/// </summary>    
public interface ISaleRepository
{
    /// <summary>
    /// Creates a new sale in the repository.
    /// </summary>
    /// <param name="sale">The sale to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale.</returns>
    Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a sale by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale if found; otherwise, null.</returns>
    Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves sales with pagination and filters.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="saleNumber">The sale number filter.</param>
    /// <param name="userId">The user identifier filter.</param>
    /// <param name="branchId">The branch identifier filter.</param>
    /// <param name="status">The sale status filter.</param>
    /// <param name="startDate">The start sale date filter.</param>
    /// <param name="endDate">The end sale date filter.</param>
    /// <param name="minTotalAmount">The minimum total amount filter.</param>
    /// <param name="maxTotalAmount">The maximum total amount filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of sales found and the total count.</returns>
    Task<(List<Sale> Sales, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? saleNumber = null,
        Guid? userId = null,
        Guid? branchId = null,
        SaleStatus? status = null,
        DateTime? startDate = null,
        DateTime? endDate = null,
        decimal? minTotalAmount = null,
        decimal? maxTotalAmount = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes an sale from the repository
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was deleted, false if not found</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing sale in the repository.
    /// </summary>
    /// <param name="sale">The sale to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated sale.</returns>
    Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default);

    /// <summary>
    /// Reactivates a cancelled sale by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale to reactivate.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the sale was reactivated; otherwise, false.</returns>
    Task<bool> ReactivateAsync(Guid id, CancellationToken cancellationToken = default);
}