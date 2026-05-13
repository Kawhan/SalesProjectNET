using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Branches.CreateBranch;

/// <summary>
/// Represents a request to create a new branch in the system.
/// </summary>
public class CreateBranchRequest
{
    /// <summary>
    /// Gets or sets the branch name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the initial status of the branch.
    /// </summary>
    public BranchStatus Status { get; set; }
}
