using AutoMapper;
using SalesProject.Domain.Entities;

namespace SalesProject.Application.Users.GetAllUsers;

/// <summary>
/// Profile for mapping between User entity and GetAllUsersResult.
/// </summary>
public class GetAllUsersProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetAllUsers operation.
    /// </summary>
    public GetAllUsersProfile()
    {
        CreateMap<User, GetAllUsersResult>();
    }
}


