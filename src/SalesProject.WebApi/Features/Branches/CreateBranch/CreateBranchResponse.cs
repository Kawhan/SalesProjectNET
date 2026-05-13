using SalesProject.Domain.Enums;

namespace SalesProject.WebApi.Features.Branches.CreateBranch;


/// <summary>
/// Represents the response returned after successfully creating a branch.
/// </summary>
public class CreateBranchResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the created branch.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the created branch name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the created branch address.
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