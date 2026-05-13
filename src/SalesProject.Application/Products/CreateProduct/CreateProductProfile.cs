using AutoMapper;
using SalesProject.Domain.Entities;

namespace SalesProject.Application.Products.CreateProduct;

/// <summary>
/// Profile for mapping between Product entity and CreateProductResult
/// </summary>
public class CreateProductProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for CreateProduct operation
    /// </summary>
    public CreateProductProfile()
    {
        CreateMap<CreateProductCommand, Product>();
        CreateMap<Product, CreateProductResult>();
    }
}

