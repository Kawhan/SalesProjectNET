using SalesProject.Domain.Common;
using SalesProject.Domain.Enums;

namespace SalesProject.Domain.Entities;

/// <summary>
/// Represents an item within a sale.
/// This entity stores product, quantity, unit price, discount and total amount information
/// for a specific product sold as part of a sale.
/// </summary>
public class SaleItem : BaseEntity
{
    /// <summary>
    /// Gets or sets the sale identifier associated with this item.
    /// </summary>
    public Guid SaleId { get; set; }

    /// <summary>
    /// Gets or sets the sale associated with this item.
    /// This navigation property represents the sale that contains this item.
    /// </summary>
    public Sale Sale { get; set; } = null!;

    /// <summary>
    /// Gets or sets the product identifier associated with this sale item.
    /// </summary>
    public Guid ProductId { get; set; }

    /// <summary>
    /// Gets or sets the product associated with this sale item.
    /// This navigation property represents the product sold in this item.
    /// </summary>
    public Product Product { get; set; } = null!;

    /// <summary>
    /// Gets or sets the quantity of identical products sold in this item.
    /// This value is used to determine the applicable discount rule.
    /// </summary>
    public int Quantity { get; set; }

    /// <summary>
    /// Gets or sets the unit price of the product at the moment of the sale.
    /// This value should be stored to preserve the sale history even if the product price changes later.
    /// </summary>
    public decimal UnitPrice { get; set; }

    /// <summary>
    /// Gets or sets the discount percentage applied to this item.
    /// For example, 0, 10 or 20 according to the quantity of identical products.
    /// </summary>
    public decimal DiscountPercentage { get; set; }

    /// <summary>
    /// Gets or sets the discount amount applied to this item.
    /// This value represents the monetary amount deducted from the item subtotal.
    /// </summary>
    public decimal Discount { get; set; }

    /// <summary>
    /// Gets or sets the total amount of this item after applying the discount.
    /// This value is usually calculated based on quantity, unit price and discount.
    /// </summary>
    public decimal TotalAmount { get; set; }

    /// <summary>
    /// Gets the date and time when the sale was created.
    /// </summary>
    public DateTime CreatedAt { get; set; }

    /// <summary>
    /// Gets the date and time of the last update to the sale's information.
    /// </summary>
    public DateTime? UpdatedAt { get; set; }

    /// <summary>
    /// Gets or sets the current status of the sale item.
    /// Indicates whether the item is active or cancelled.
    /// </summary>
    public SaleItemStatus Status { get; set; }

    public SaleItem()
    {
        Status = SaleItemStatus.Active;
        CreatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Updates the sale item information and recalculates its amounts.
    /// </summary>
    /// <param name="quantity">The updated quantity.</param>
    /// <param name="unitPrice">The updated unit price.</param>
    public void Update(int quantity, decimal unitPrice)
    {
        Quantity = quantity;
        UnitPrice = unitPrice;
        Status = SaleItemStatus.Active;

        Recalculate();
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Cancels the sale item.
    /// </summary>
    public void Cancel()
    {
        Status = SaleItemStatus.Cancelled;
        UpdatedAt = DateTime.UtcNow;
    }

    /// <summary>
    /// Recalculates the sale item amounts.
    /// <summary>
    public void Recalculate()
    {
        var subtotalAmount = Quantity * UnitPrice;

        DiscountPercentage = GetDiscountPercentage(Quantity);
        Discount = subtotalAmount * (DiscountPercentage / 100);
        TotalAmount = subtotalAmount - Discount;
    }

    /// <summary>
    /// Get the new discount percentage.
    /// <summary>
    private static decimal GetDiscountPercentage(int quantity)
    {
        if (quantity >= 10 && quantity <= 20)
            return 20;

        if (quantity >= 4)
            return 10;

        return 0;
    }
}