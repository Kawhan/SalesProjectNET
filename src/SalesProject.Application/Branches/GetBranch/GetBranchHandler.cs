using AutoMapper;
using MediatR;
using SalesProject.Domain.Repositories;

using FluentValidation;

namespace SalesProject.Application.Branches.GetBranch;


/// <summary>
/// Handles the GetBranchCommand request.
/// </summary>
public class GetBranchHandler : IRequestHandler<GetBranchCommand, GetBranchResult>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetBranchHandler.
    /// </summary>
    /// <param name="branchRepository">The branch repository instance.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetBranchHandler(IBranchRepository branchRepository, IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetBranchCommand request.
    /// </summary>
    /// <param name="request">The GetBranch command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The branch details.</returns>
    public async Task<GetBranchResult> Handle(
        GetBranchCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new GetBranchCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var branch = await _branchRepository.GetByIdAsync(request.Id, cancellationToken);

        if (branch is null)
            throw new KeyNotFoundException($"Branch with id {request.Id} was not found.");

        return _mapper.Map<GetBranchResult>(branch);
    }
}
