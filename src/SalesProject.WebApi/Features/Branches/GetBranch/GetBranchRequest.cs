namespace SalesProject.WebApi.Features.Branches.GetBranch;

/// <summary>
/// Represents a request to retrieve a branch by its ID.
/// </summary>
public class GetBranchRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the branch.
    /// </summary>
    public Guid Id { get; set; }
}