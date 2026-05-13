using MediatR;

namespace SalesProject.Application.Sales.GetSale;

/// <summary>
/// Command for retrieving a sale by its ID
/// </summary>
public record class GetSaleCommand : IRequest<GetSaleResult>
{
    /// <summary>
    /// The unique identifier of the product to retrieve
    /// </summary>
    public Guid Id { get; }

    /// <summary>
    /// Initializes a new instance of GetProductCommand
    /// </summary>
    /// <param name="id">The ID of the product to retrieve</param>
    public GetSaleCommand(Guid id)
    {
        Id = id;
    }
}

