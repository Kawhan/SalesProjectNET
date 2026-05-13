using SalesProject.Domain.Enums;

namespace SalesProject.Application.Products.CreateProduct;


/// <summary>
/// Represents the response returned after successfully creating a new product.
/// </summary>
/// <remarks>
/// This response contains the unique identifier, name and current price of the newly created product,
/// which can be used for subsequent operations or reference.
/// </remarks>
public class CreateProductResult
{
    /// <summary>
    /// Gets or sets the unique identifier of the newly created product.
    /// </summary>
    /// <value>A GUID that uniquely identifies the created product in the system.</value>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the name of the newly created product.
    /// </summary>
    /// <value>The product name registered in the system.</value>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current price of the newly created product.
    /// </summary>
    /// <value>The active price of the product at the moment of creation.</value>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// The current status of the product
    /// </summary>
    public ProductStatus Status { get; set; }
}

