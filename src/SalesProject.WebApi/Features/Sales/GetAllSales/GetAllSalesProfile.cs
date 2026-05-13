using AutoMapper;
using SalesProject.Application.Sales.GetAllSales;

namespace SalesProject.WebApi.Features.Sales.GetAllSales;

/// <summary>
/// Profile for mapping GetAllSales feature models.
/// </summary>
public class GetAllSalesProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetAllSales feature.
    /// </summary>
    public GetAllSalesProfile()
    {
        CreateMap<GetAllSalesRequest, GetAllSalesCommand>();

        CreateMap<GetAllSalesResult, GetAllSalesResponse>();
        CreateMap<GetAllSalesItemResult, GetAllSalesItemResponse>();
    }
}