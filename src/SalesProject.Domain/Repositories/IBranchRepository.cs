
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Domain.Repositories;

/// <summary>
/// Defines repository operations for managing branches.
/// </summary>
public interface IBranchRepository
{
    /// <summary>
    /// Creates a new branch in the repository.
    /// </summary>
    /// <param name="branch">The branch to create.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created branch.</returns>
    Task<Branch> CreateAsync(Branch branch, CancellationToken cancellationToken = default);

    /// <summary>
    /// Retrieves a branch by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the branch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The branch if found, null otherwise.</returns>
    Task<Branch?> GetByIdAsync(Guid id, CancellationToken cancellationToken = default);

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
    Task<(List<Branch> Branches, int TotalCount)> GetAllAsync(
        int pageNumber,
        int pageSize,
        string? name = null,
        string? address = null,
        BranchStatus? status = null,
        CancellationToken cancellationToken = default);

    /// <summary>
    /// Deletes a branch by its unique identifier.
    /// </summary>
    /// <param name="id">The unique identifier of the branch to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>True if the branch was deleted; otherwise, false.</returns>
    Task<bool> DeleteAsync(Guid id, CancellationToken cancellationToken = default);

    /// <summary>
    /// Updates an existing branch in the repository.
    /// </summary>
    /// <param name="branch">The branch to update.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated branch.</returns>
    Task<Branch> UpdateAsync(Branch branch, CancellationToken cancellationToken = default);
}
