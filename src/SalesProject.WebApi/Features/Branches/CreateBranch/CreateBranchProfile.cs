using AutoMapper;
using SalesProject.Application.Branches.CreateBranch;

namespace SalesProject.WebApi.Features.Branches.CreateBranch;

/// <summary>
/// Profile for mapping CreateBranch feature models.
/// </summary>
public class CreateBranchProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateBranch feature.
    /// </summary>
    public CreateBranchProfile()
    {
        CreateMap<CreateBranchRequest, CreateBranchCommand>();
        CreateMap<CreateBranchResult, CreateBranchResponse>();
    }
}