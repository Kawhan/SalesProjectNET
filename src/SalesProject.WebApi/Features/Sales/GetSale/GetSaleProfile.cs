using AutoMapper;
using SalesProject.Application.Sales.GetSale;

namespace SalesProject.WebApi.Features.Sales.GetSale;

/// <summary>
/// Profile for mapping GetSale feature models.
/// </summary>
public class GetSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetSale feature.
    /// </summary>
    public GetSaleProfile()
    {
        CreateMap<Guid, Application.Sales.GetSale.GetSaleCommand>()
            .ConstructUsing(id => new Application.Sales.GetSale.GetSaleCommand(id));

        CreateMap<GetSaleResult, GetSaleResponse>();
        CreateMap<GetSaleItemResult, GetSaleItemResponse>();
        CreateMap<GetSaleUserResult, GetSaleUserResponse>();
        CreateMap<GetSaleBranchResult, GetSaleBranchResponse>();
        CreateMap<GetSaleProductResult, GetSaleProductResponse>();
    }
}
