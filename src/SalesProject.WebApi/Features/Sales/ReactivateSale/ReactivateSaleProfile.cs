using AutoMapper;

namespace SalesProject.WebApi.Features.Sales.ReactivateSale;

public class ReactivateSaleProfile : Profile
{
    /// <summary>
    /// Initializes the mappings for ReactivateSale feature
    /// </summary>
    public ReactivateSaleProfile()
    {
        CreateMap<Guid, Application.Sales.ReactivateSale.ReactivateSaleCommand>()
            .ConstructUsing(id => new Application.Sales.ReactivateSale.ReactivateSaleCommand(id));
    }
}

