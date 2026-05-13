using MediatR;
using SalesProject.Domain.Enums;

namespace SalesProject.Application.Branches.GetAllBranches;


/// <summary>
/// Command for retrieving branches with pagination and filters.
/// </summary>
public class GetAllBranchesCommand : IRequest<GetAllBranchesPaginatedResult>
{
    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the branch name filter.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the branch address filter.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the branch status filter.
    /// </summary>
    public BranchStatus? Status { get; set; }
}