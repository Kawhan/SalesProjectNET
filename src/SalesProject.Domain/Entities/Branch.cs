using SalesProject.Domain.Common;
using SalesProject.Domain.Enums;

namespace SalesProject.Domain.Entities;

/// <summary>
/// Represents a branch where sales can be made.
/// This entity stores branch identification and location information,
/// and keeps the relationship with the sales made in this branch.
/// </summary>
public class Branch : BaseEntity
{
    /// <summary>
    /// Gets or sets the branch name.
    /// This value identifies the branch from a business perspective.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch address.
    /// This value can be null when the branch address is not provided.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the list of sales associated with this branch.
    /// A branch can have multiple sales.
    /// </summary>
    public List<Sale> Sales { get; set; } = new();

    /// <summary>
    /// Gets the branch's current status.
    /// Indicates whether the branch is active or inactive.
    /// </summary>
    public BranchStatus Status { get; set; }

    /// <summary>
    /// Gets the date and time when the branch was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the branch's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    // <summary>
    /// Initializes a new instance of the branch class.
    /// </summary>
    public Branch()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the branch.
    /// Changes the user's status to Active.
    /// </summary>
    public void Activate()
    {
        Status = BranchStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the branch.
    /// Changes the branch's status to Inactive.
    /// </summary>
    public void Deactivate()
    {
        Status = BranchStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the branch information.
    /// </summary>
    /// <param name="name">The updated branch name.</param>
    /// <param name="address">The updated branch address.</param>
    /// <param name="status">The updated branch status.</param>
    public void Update(string name, string? address, BranchStatus status)
    {
        Name = name;
        Address = address;
        Status = status;
        UpdatedAt = DateTime.UtcNow;
    }
}