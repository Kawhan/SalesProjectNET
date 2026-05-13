using AutoMapper;
using SalesProject.Application.Products.GetAllProducts;

namespace SalesProject.WebApi.Features.Products.GetAllProducts;

/// <summary>
/// Profile for mapping GetAllProducts feature models.
/// </summary>
public class GetAllProductsProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetAllProducts feature.
    /// </summary>
    public GetAllProductsProfile()
    {
        CreateMap<GetAllProductsRequest, GetAllProductsCommand>();
        CreateMap<GetAllProductsResult, GetAllProductsResponse>();
    }
}