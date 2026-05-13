using AutoMapper;
using SalesProject.Application.Branches.GetAllBranches;

namespace SalesProject.WebApi.Features.Branches.GetAllBranches;

/// <summary>
/// Profile for mapping GetAllBranches feature models.
/// </summary>
public class GetAllBranchesProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetAllBranches feature.
    /// </summary>
    public GetAllBranchesProfile()
    {
        CreateMap<GetAllBranchesRequest, GetAllBranchesCommand>();
        CreateMap<GetAllBranchesResult, GetAllBranchesResponse>();
    }
}
