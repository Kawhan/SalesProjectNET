using Microsoft.EntityFrameworkCore;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Repositories;

namespace SalesProject.ORM.Repositories;

/// <summary>
/// Repository implementation for managing sales.
/// </summary>
public class SaleRepository : ISaleRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of the SaleRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public SaleRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new sale in the database.
    /// </summary>
    /// <param name="sale">The sale to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created sale.</returns>
    public async Task<Sale> CreateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        await _context.Sales.AddAsync(sale, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    /// <summary>
    /// Retrieves a sale by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the sale.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The sale if found; otherwise, null.</returns>
    public async Task<Sale?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Sales
        .Include(sale => sale.User)
        .Include(sale => sale.Branch)
        .Include(sale => sale.Items)
            .ThenInclude(item => item.Product)
        .FirstOrDefaultAsync(sale => sale.Id == id, cancellationToken);
    }

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
    public async Task<(List<Sale> Sales, int TotalCount)> GetAllAsync(
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
        CancellationToken cancellationToken = default)
    {
        var query = _context.Sales
            .Include(sale => sale.User)
            .Include(sale => sale.Branch)
            .Include(sale => sale.Items)
                .ThenInclude(item => item.Product)
            .AsQueryable();

        if (!string.IsNullOrWhiteSpace(saleNumber))
        {
            var normalizedSaleNumber = saleNumber.ToLower();

            query = query.Where(sale =>
                sale.SaleNumber.ToLower().Contains(normalizedSaleNumber));
        }

        if (userId.HasValue)
        {
            query = query.Where(sale =>
                sale.UserId == userId.Value);
        }

        if (branchId.HasValue)
        {
            query = query.Where(sale =>
                sale.BranchId == branchId.Value);
        }

        if (status.HasValue)
        {
            query = query.Where(sale =>
                sale.Status == status.Value);
        }

        if (startDate.HasValue)
        {
            query = query.Where(sale =>
                sale.SaleDate >= startDate.Value);
        }

        if (endDate.HasValue)
        {
            query = query.Where(sale =>
                sale.SaleDate <= endDate.Value);
        }

        if (minTotalAmount.HasValue)
        {
            query = query.Where(sale =>
                sale.TotalAmount >= minTotalAmount.Value);
        }

        if (maxTotalAmount.HasValue)
        {
            query = query.Where(sale =>
                sale.TotalAmount <= maxTotalAmount.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var sales = await query
            .OrderByDescending(sale => sale.SaleDate)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (sales, totalCount);
    }


    /// <summary>
    /// Deletes an sale from the repository
    /// </summary>
    /// <param name="id">The unique identifier of the sale to delete</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>True if the sale was deleted, false if not found</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);
        if (sale == null)
            return false;

        sale.Deactivate();

        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return true;
    }
    public async Task<Sale> UpdateAsync(Sale sale, CancellationToken cancellationToken = default)
    {
        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);
        return sale;
    }

    public async Task<bool> ReactivateAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var sale = await GetByIdAsync(id, cancellationToken);

        if (sale is null)
            return false;

        if (sale.Status == SaleStatus.Active)
            return true;

        sale.Activate();

        _context.Sales.Update(sale);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }
}
