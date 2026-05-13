using MediatR;

namespace SalesProject.Application.Branches.GetBranch;

/// <summary>
/// Command for retrieving a branch by its ID.
/// </summary>
public record GetBranchCommand : IRequest<GetBranchResult>
{
    /// <summary>
    /// The unique identifier of the branch to retrieve.
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of GetBranchCommand.
    /// </summary>
    /// <param name="id">The ID of the branch to retrieve.</param>
    public GetBranchCommand(Guid id)
    {
        Id = id;
    }
}