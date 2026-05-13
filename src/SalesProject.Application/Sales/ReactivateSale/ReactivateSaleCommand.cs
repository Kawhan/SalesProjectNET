using MediatR;

namespace SalesProject.Application.Sales.ReactivateSale;

/// <summary>
/// Command for reactivating a cancelled sale by its ID.
/// </summary>
public record ReactivateSaleCommand : IRequest<ReactivateSaleResponse>
{
    /// <summary>
    /// The unique identifier of the sale to reactivate.
    /// </summary>
    public Guid Id { get; }

    public ReactivateSaleCommand(Guid id)
    {
        Id = id;
    }
}