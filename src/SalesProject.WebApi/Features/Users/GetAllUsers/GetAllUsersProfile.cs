using AutoMapper;
using SalesProject.Application.Users.GetAllUsers;

namespace SalesProject.WebApi.Features.Users.GetAllUsers;

/// <summary>
/// Profile for mapping GetAllUsers feature models.
/// </summary>
public class GetAllUsersProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetAllUsers feature.
    /// </summary>
    public GetAllUsersProfile()
    {
        CreateMap<GetAllUsersRequest, GetAllUsersCommand>();
        CreateMap<GetAllUsersResult, GetAllUsersResponse>();
    }
}
