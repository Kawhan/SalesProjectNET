using AutoMapper;
using SalesProject.Domain.Entities;

namespace SalesProject.Application.Branches.UpdateBranch;

/// <summary>
/// Profile for mapping between Branch entity and UpdateBranchResult.
/// </summary>
public class UpdateBranchProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for UpdateBranch operation.
    /// </summary>
    public UpdateBranchProfile()
    {
        CreateMap<Branch, UpdateBranchResult>();
    }
}