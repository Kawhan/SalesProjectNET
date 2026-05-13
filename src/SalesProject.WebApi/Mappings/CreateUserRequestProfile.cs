using SalesProject.Application.Users.CreateUser;
using SalesProject.WebApi.Features.Users.CreateUser;

using AutoMapper;


namespace SalesProject.WebApi.Mappings;

public class CreateUserRequestProfile : Profile
{
    public CreateUserRequestProfile()
    {
        CreateMap<CreateUserRequest, CreateUserCommand>();
    }
}
