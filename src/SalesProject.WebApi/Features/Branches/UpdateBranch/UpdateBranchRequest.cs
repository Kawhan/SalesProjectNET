using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Branches.UpdateBranch;

/// <summary>
/// Represents a request to update an existing branch in the system.
/// </summary>
public class UpdateBranchRequest
{
    /// <summary>
    /// Gets or sets the updated branch name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated branch address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the updated branch status.
    /// </summary>
    public BranchStatus Status { get; set; }
}