using AutoMapper;
using MediatR;
using SalesProject.Domain.Repositories;

namespace SalesProject.Application.Branches.GetAllBranches;

/// <summary>
/// Handler for processing GetAllBranchesCommand requests.
/// </summary>
public class GetAllBranchesHandler : IRequestHandler<GetAllBranchesCommand, GetAllBranchesPaginatedResult>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetAllBranchesHandler.
    /// </summary>
    /// <param name="branchRepository">The branch repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetAllBranchesHandler(
        IBranchRepository branchRepository,
        IMapper mapper)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetAllBranchesCommand request.
    /// </summary>
    /// <param name="request">The GetAllBranches command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The paginated list of branches.</returns>
    public async Task<GetAllBranchesPaginatedResult> Handle(
        GetAllBranchesCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _branchRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            request.Name,
            request.Address,
            request.Status,
            cancellationToken);

        var branches = _mapper.Map<List<GetAllBranchesResult>>(result.Branches);

        return new GetAllBranchesPaginatedResult
        {
            Data = branches,
            CurrentPage = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = (int)Math.Ceiling(result.TotalCount / (double)request.PageSize)
        };
    }
}