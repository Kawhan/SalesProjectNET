using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Branches.Events;


namespace SalesProject.IoC.Messaging.Handlers.Branches;

/// <summary>
/// Handles BranchCreatedEvent messages.
/// </summary>
public class BranchCreatedEventHandler : IHandleMessages<BranchCreatedEvent>
{
    private readonly ILogger<BranchCreatedEventHandler> _logger;

    public BranchCreatedEventHandler(ILogger<BranchCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(BranchCreatedEvent message)
    {
        _logger.LogInformation(
            "Branch created. Id: {Id}, Name: {Name}, Adress: {Address}, Status: {Status}, CreatedAt: {CreatedAt}",
            message.Id,
            message.Name,
            message.Address,
            message.Status,
            message.CreatedAt);

        return Task.CompletedTask;
    }
}

