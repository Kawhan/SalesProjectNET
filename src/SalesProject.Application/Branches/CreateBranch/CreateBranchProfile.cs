using AutoMapper;
using SalesProject.Domain.Entities;

namespace SalesProject.Application.Branches.CreateBranch;

/// <summary>
/// Profile for mapping between Branch entity and CreateBranch operation models.
/// </summary>
public class CreateBranchProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateBranch operation.
    /// </summary>
    public CreateBranchProfile()
    {
        CreateMap<CreateBranchCommand, Branch>();
        CreateMap<Branch, CreateBranchResult>();
    }
}