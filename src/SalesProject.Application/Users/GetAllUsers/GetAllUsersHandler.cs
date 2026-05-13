using AutoMapper;
using MediatR;
using SalesProject.Domain.Repositories;

namespace SalesProject.Application.Users.GetAllUsers;

/// <summary>
/// Handler for processing GetAllUsersCommand requests.
/// </summary>
public class GetAllUsersHandler : IRequestHandler<GetAllUsersCommand, GetAllUsersPaginatedResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetAllUsersHandler.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetAllUsersHandler(
        IUserRepository userRepository,
        IMapper mapper)
    {
        _userRepository = userRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetAllUsersCommand request.
    /// </summary>
    /// <param name="request">The GetAllUsers command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The paginated list of users.</returns>
    public async Task<GetAllUsersPaginatedResult> Handle(
        GetAllUsersCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _userRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            request.Username,
            request.Email,
            request.Phone,
            request.Role,
            request.Status,
            cancellationToken);

        var users = _mapper.Map<List<GetAllUsersResult>>(result.Users);

        return new GetAllUsersPaginatedResult
        {
            Data = users,
            CurrentPage = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = (int)Math.Ceiling(result.TotalCount / (double)request.PageSize)
        };
    }
}