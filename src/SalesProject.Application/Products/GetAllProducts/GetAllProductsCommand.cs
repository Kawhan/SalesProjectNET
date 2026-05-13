using MediatR;
using SalesProject.Domain.Enums;

namespace SalesProject.Application.Products.GetAllProducts;

/// <summary>
/// Command for retrieving products with pagination and filters.
/// </summary>
public class GetAllProductsCommand : IRequest<GetAllProductsPaginatedResult>
{
    /// <summary>
    /// Gets or sets the page number.
    /// </summary>
    public int PageNumber { get; set; } = 1;

    /// <summary>
    /// Gets or sets the number of items per page.
    /// </summary>
    public int PageSize { get; set; } = 10;

    /// <summary>
    /// Gets or sets the product name filter.
    /// </summary>
    public string? Name { get; set; }

    /// <summary>
    /// Gets or sets the minimum product price filter.
    /// </summary>
    public decimal? MinPrice { get; set; }

    /// <summary>
    /// Gets or sets the maximum product price filter.
    /// </summary>
    public decimal? MaxPrice { get; set; }

    /// <summary>
    /// Gets or sets the product status filter.
    /// </summary>
    public ProductStatus? Status { get; set; }
}