using AutoMapper;
using SalesProject.Domain.Entities;

namespace SalesProject.Application.Sales.GetSale;

/// <summary>
/// Profile for mapping between Sale entity and GetSaleResponse
/// </summary>
public class GetSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetSale operation.
    /// </summary>
    public GetSaleProfile()
    {
        CreateMap<Sale, GetSaleResult>();
        CreateMap<SaleItem, GetSaleItemResult>();
        CreateMap<User, GetSaleUserResult>();
        CreateMap<Branch, GetSaleBranchResult>();
        CreateMap<Product, GetSaleProductResult>();
    }
}

