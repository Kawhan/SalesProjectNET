using AutoMapper;
using SalesProject.Domain.Entities;

namespace SalesProject.Application.Branches.GetBranch;

/// <summary>
/// Profile for mapping between Branch entity and GetBranchResult.
/// </summary>
public class GetBranchProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetBranch operation.
    /// </summary>
    public GetBranchProfile()
    {
        CreateMap<Branch, GetBranchResult>();
    }
}