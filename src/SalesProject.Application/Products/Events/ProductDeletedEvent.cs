using SalesProject.Domain.Enums;

namespace SalesProject.Application.Products.Events;

public class ProductDeletedEvent
{
    /// <summary>
    /// Gets or sets the product identifier.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the product name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current price.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// Gets or sets the product status.
    /// </summary>
    public ProductStatus Status { get; set; }

    /// <summary>
    /// Gets or sets the date when the product was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets or sets the date when the product was Deleted.
    /// </summary>
    public DateTime UpdatedAt { get; set; }
}
