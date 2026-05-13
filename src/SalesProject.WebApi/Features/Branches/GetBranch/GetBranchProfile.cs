using AutoMapper;
using SalesProject.Application.Branches.GetBranch;

namespace SalesProject.WebApi.Features.Branches.GetBranch;

/// <summary>
/// Profile for mapping GetBranch feature models.
/// </summary>
public class GetBranchProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetBranch feature.
    /// </summary>
    public GetBranchProfile()
    {
        CreateMap<Guid, GetBranchCommand>()
            .ConstructUsing(id => new GetBranchCommand(id));

        CreateMap<GetBranchResult, GetBranchResponse>();
    }
}