using SalesProject.Domain.Enums;

namespace SalesProject.Application.Sales.GetSale;

/// <summary>
/// Represents the branch returned in the GetSale operation.
/// </summary>
public class GetSaleBranchResult
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
}
