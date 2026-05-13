using AutoMapper;
using MediatR;
using SalesProject.Domain.Repositories;

namespace SalesProject.Application.Sales.GetAllSales;

/// <summary>
/// Handler for processing GetAllSalesCommand requests.
/// </summary>
public class GetAllSalesHandler : IRequestHandler<GetAllSalesCommand, GetAllSalesPaginatedResult>
{
    private readonly ISaleRepository _saleRepository;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of GetAllSalesHandler.
    /// </summary>
    /// <param name="saleRepository">The sale repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    public GetAllSalesHandler(ISaleRepository saleRepository, IMapper mapper)
    {
        _saleRepository = saleRepository;
        _mapper = mapper;
    }

    /// <summary>
    /// Handles the GetAllSalesCommand request.
    /// </summary>
    /// <param name="request">The GetAllSales command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The paginated list of sales.</returns>
    public async Task<GetAllSalesPaginatedResult> Handle(
        GetAllSalesCommand request,
        CancellationToken cancellationToken)
    {
        var result = await _saleRepository.GetAllAsync(
            request.PageNumber,
            request.PageSize,
            request.SaleNumber,
            request.UserId,
            request.BranchId,
            request.Status,
            request.StartDate,
            request.EndDate,
            request.MinTotalAmount,
            request.MaxTotalAmount,
            cancellationToken);

        var sales = _mapper.Map<List<GetAllSalesResult>>(result.Sales);

        return new GetAllSalesPaginatedResult
        {
            Data = sales,
            CurrentPage = request.PageNumber,
            PageSize = request.PageSize,
            TotalCount = result.TotalCount,
            TotalPages = (int)Math.Ceiling(result.TotalCount / (double)request.PageSize)
        };
    }
}
