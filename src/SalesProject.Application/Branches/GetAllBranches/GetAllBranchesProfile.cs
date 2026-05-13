using AutoMapper;
using SalesProject.Domain.Entities;

namespace SalesProject.Application.Branches.GetAllBranches;

/// <summary>
/// Profile for mapping between Branch entity and GetAllBranchesResult.
/// </summary>
public class GetAllBranchesProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetAllBranches operation.
    /// </summary>
    public GetAllBranchesProfile()
    {
        CreateMap<Branch, GetAllBranchesResult>();
    }
}