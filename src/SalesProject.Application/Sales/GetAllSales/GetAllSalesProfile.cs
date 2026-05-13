using AutoMapper;
using SalesProject.Domain.Entities;

namespace SalesProject.Application.Sales.GetAllSales;

/// <summary>
/// Profile for mapping between Sale entity and GetAllSalesResult.
/// </summary>
public class GetAllSalesProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetAllSales operation.
    /// </summary>
    public GetAllSalesProfile()
    {
        CreateMap<Sale, GetAllSalesResult>()
            .ForMember(dest => dest.ItemsCount, opt => opt.MapFrom(src => src.Items.Count));

        CreateMap<SaleItem, GetAllSalesItemResult>()
            .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product.Name));
    }
}
