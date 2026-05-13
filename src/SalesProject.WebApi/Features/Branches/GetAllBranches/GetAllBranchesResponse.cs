using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Branches.GetAllBranches;

/// <summary>
/// Response model for the GetAllBranches operation.
/// </summary>
public class GetAllBranchesResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the branch.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the branch name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the current status of the branch.
    /// </summary>
    public BranchStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the branch was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date and time when the branch was last updated.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }
}
