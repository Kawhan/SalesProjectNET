using AutoMapper;
using SalesProject.Application.Branches.UpdateBranch;

namespace SalesProject.WebApi.Features.Branches.UpdateBranch;

/// <summary>
/// Profile for mapping UpdateBranch feature models.
/// </summary>
public class UpdateBranchProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateBranch feature.
    /// </summary>
    public UpdateBranchProfile()
    {
        CreateMap<UpdateBranchRequest, UpdateBranchCommand>();
        CreateMap<UpdateBranchResult, UpdateBranchResponse>();
    }
}