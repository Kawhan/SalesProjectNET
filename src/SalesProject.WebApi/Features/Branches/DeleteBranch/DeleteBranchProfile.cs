using AutoMapper;
using SalesProject.Application.Branches.DeleteBranch;

namespace SalesProject.WebApi.Features.Branches.DeleteBranch;

/// <summary>
/// Profile for mapping DeleteBranch feature models.
/// </summary>
public class DeleteBranchProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for DeleteBranch feature.
    /// </summary>
    public DeleteBranchProfile()
    {
        CreateMap<Guid, DeleteBranchCommand>()
            .ConstructUsing(id => new DeleteBranchCommand(id));
    }
}
