using AutoMapper;
using SalesProject.Application.Users.UpdateUser;

namespace SalesProject.WebApi.Features.Users.UpdateUser;

/// <summary>
/// Profile for mapping UpdateUser feature models.
/// </summary>
public class UpdateUserProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateUser feature.
    /// </summary>
    public UpdateUserProfile()
    {
        CreateMap<UpdateUserRequest, UpdateUserCommand>();
        CreateMap<UpdateUserResult, UpdateUserResponse>();
    }
}


