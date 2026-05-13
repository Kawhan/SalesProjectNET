namespace SalesProject.WebApi.Features.Branches.DeleteBranch;

/// <summary>
/// Represents a request to delete a branch by its ID.
/// </summary>
public class DeleteBranchRequest
{
    /// <summary>
    /// Gets or sets the unique identifier of the branch to delete.
    /// </summary>
    public Guid Id { get; set; }
}
