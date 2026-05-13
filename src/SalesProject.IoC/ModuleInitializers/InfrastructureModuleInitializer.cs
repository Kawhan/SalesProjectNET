using SalesProject.Application.Branches.Events;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Products.Events;
using SalesProject.Application.Sales.Events;
using SalesProject.Application.Users.Events;
using SalesProject.Domain.Repositories;
using SalesProject.IoC.Messaging;
using SalesProject.IoC.Messaging.Handlers.Sales;
using SalesProject.ORM;
using SalesProject.ORM.Repositories;


using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Rebus.Config;

namespace SalesProject.IoC.ModuleInitializers;

public class InfrastructureModuleInitializer : IModuleInitializer
{
    public void Initialize(WebApplicationBuilder builder)
    {
        builder.Services.AddScoped<DbContext>(provider => provider.GetRequiredService<DefaultContext>());
        builder.Services.AddScoped<IUserRepository, UserRepository>();
        builder.Services.AddScoped<IProductRepository, ProductRepository>();
        builder.Services.AddScoped<ISaleRepository, SaleRepository>();
        builder.Services.AddScoped<IBranchRepository, BranchRepository>();

        builder.Services.AddScoped<IMessageBusService, RebusMessageBusService>();

        builder.Services.AutoRegisterHandlersFromAssemblyOf<SaleCreatedEventHandler>();

        builder.Services.AddRebus(
            configure => configure
                .Logging(l => l.Console())
                .Transport(t => t.UseRabbitMq(
                    builder.Configuration.GetConnectionString("RabbitMq"),
                    "salesproject-queue")),
            onCreated: async bus =>
            {
                await bus.Subscribe<SaleCreatedEvent>();
                await bus.Subscribe<SaleCancelledEvent>();
                await bus.Subscribe<SaleModifiedEvent>();
                await bus.Subscribe<SaleItemsCancelledEvent>();
                await bus.Subscribe<SaleReactivatedEvent>();

                await bus.Subscribe<ProductCreatedEvent>();
                await bus.Subscribe<ProductDeletedEvent>();
                await bus.Subscribe<ProductModifiedEvent>();

                await bus.Subscribe<BranchCreatedEvent>();
                await bus.Subscribe<BranchModifiedEvent>();
                await bus.Subscribe<BranchDeletedEvent>();

                await bus.Subscribe<UserCreatedEvent>();
                await bus.Subscribe<UserDeletedEvent>();
                await bus.Subscribe<UserModifiedEvent>();
            });
    }
}
