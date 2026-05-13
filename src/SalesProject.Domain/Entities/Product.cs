using SalesProject.Domain.Common;
using SalesProject.Domain.Enums;


namespace SalesProject.Domain.Entities;

/// <summary>
/// Represents a product available for sale.
/// This entity stores product information such as name and current price,
/// and keeps the relationship with sale items where the product was sold.
/// </summary>
public class Product : BaseEntity
{
    /// <summary>
    /// Gets or sets the product name.
    /// This value identifies the product from a business perspective.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the current price of the product.
    /// This value represents the active product price and may change over time.
    /// Sale items should store the unit price used at the moment of the sale
    /// to preserve the sale history.
    /// </summary>
    public decimal CurrentPrice { get; set; }

    /// <summary>
    /// Gets the product's current status.
    /// Indicates whether the product is active or inactive.
    /// </summary>
    public ProductStatus Status { get; set; }

    /// <summary>
    /// Gets the date and time when the product was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the product's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the list of sale items associated with this product.
    /// A product can be present in multiple sale items across different sales.
    /// </summary>
    public List<SaleItem> SaleItems { get; set; } = new();

    public Product()
    {
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Activates the product.
    /// Changes the product's status to Active.
    /// </summary>
    public void Activate()
    {
        Status = ProductStatus.Active;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Deactivates the product.
    /// Changes the product's status to Inactive.
    /// </summary>
    public void Deactivate()
    {
        Status = ProductStatus.Inactive;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the date and time of the last product modification.
    /// </summary>
    public void UpdateTimestamp()
    {
        UpdatedAt = DateTime.UtcNow;
    }
}