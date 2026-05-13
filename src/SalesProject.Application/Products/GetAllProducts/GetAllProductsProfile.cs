using AutoMapper;
using SalesProject.Domain.Entities;

namespace SalesProject.Application.Products.GetAllProducts;

/// <summary>
/// Profile for mapping between Product entity and GetAllProductsResult.
/// </summary>
public class GetAllProductsProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for GetAllProducts operation.
    /// </summary>
    public GetAllProductsProfile()
    {
        CreateMap<Product, GetAllProductsResult>();
    }
}