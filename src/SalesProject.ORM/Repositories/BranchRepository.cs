using Microsoft.EntityFrameworkCore;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Domain.Repositories;

namespace SalesProject.ORM.Repositories;

/// <summary>
/// Repository implementation for managing branches.
/// </summary>
public class BranchRepository : IBranchRepository
{
    private readonly DefaultContext _context;

    /// <summary>
    /// Initializes a new instance of the BranchRepository class.
    /// </summary>
    /// <param name="context">The database context.</param>
    public BranchRepository(DefaultContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Creates a new branch in the database.
    /// </summary>
    /// <param name="branch">The branch to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created branch.</returns>
    public async Task<Branch> CreateAsync(
        Branch branch,
        CancellationToken cancellationToken = default)
    {
        await _context.Branches.AddAsync(branch, cancellationToken);
        await _context.SaveChangesAsync(cancellationToken);
        return branch;
    }

    /// <summary>
    /// Retrieves a branch by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the branch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The branch if found, null otherwise.</returns>
    public async Task<Branch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default)
    {
        return await _context.Branches
            .FirstOrDefaultAsync(branch => branch.Id == id, cancellationToken);
    }

    /// <summary>
    /// Retrieves branches with pagination and filters.
    /// </summary>
    /// <param name="pageNumber">The page number.</param>
    /// <param name="pageSize">The number of items per page.</param>
    /// <param name="name">The branch name filter.</param>
    /// <param name="address">The branch address filter.</param>
    /// <param name="status">The branch status filter.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The list of branches found and the total count.</returns>
    public async Task<(List<Branch> Branches, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? name = null,
        string? address = null,
        BranchStatus? status = null,
        CancellationToken cancellationToken = default)
    {
        var query = _context.Branches.AsQueryable();

        if (!string.IsNullOrWhiteSpace(name))
        {
            var normalizedName = name.ToLower();

            query = query.Where(branch =>
                branch.Name.ToLower().Contains(normalizedName));
        }

        if (!string.IsNullOrWhiteSpace(address))
        {
            var normalizedAddress = address.ToLower();

            query = query.Where(branch =>
                branch.Address != null &&
                branch.Address.ToLower().Contains(normalizedAddress));
        }

        if (status.HasValue)
        {
            query = query.Where(branch =>
                branch.Status == status.Value);
        }

        var totalCount = await query.CountAsync(cancellationToken);

        var branches = await query
            .OrderBy(branch => branch.Name)
            .Skip((pageNumber - 1) * pageSize)
            .Take(pageSize)
            .ToListAsync(cancellationToken);

        return (branches, totalCount);
    }

    /// <summary>
    /// Deletes a branch by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the branch to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the branch was deleted; otherwise, false.</returns>
    public async Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default)
    {
        var branch = await GetByIdAsync(id, cancellationToken);

        if (branch is null)
            return false;

        branch.Deactivate();

        _context.Branches.Update(branch);
        await _context.SaveChangesAsync(cancellationToken);

        return true;
    }

    /// <summary>
    /// Updates an existing branch in the database.
    /// </summary>
    /// <param name="branch">The branch to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated branch.</returns>
    public async Task<Branch> UpdateAsync(
        Branch branch,
        CancellationToken cancellationToken = default)
    {
        _context.Update(branch);
        await _context.SaveChangesAsync(cancellationToken);
        return branch;
    }
}


