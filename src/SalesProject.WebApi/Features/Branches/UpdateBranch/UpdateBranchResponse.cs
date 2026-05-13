using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Branches.UpdateBranch;

/// <summary>
/// Represents the response returned after successfully updating a branch.
/// </summary>
public class UpdateBranchResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the updated branch.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the updated branch name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated branch address.
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