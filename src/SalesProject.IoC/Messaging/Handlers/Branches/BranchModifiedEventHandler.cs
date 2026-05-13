using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Branches.Events;


namespace SalesProject.IoC.Messaging.Handlers.Branches;

/// <summary>
/// Handles BranchModifiedEvent messages.
/// </summary>
public class BranchModifiedEventHandler : IHandleMessages<BranchModifiedEvent>
{
    private readonly ILogger<BranchModifiedEventHandler> _logger;

    public BranchModifiedEventHandler(ILogger<BranchModifiedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(BranchModifiedEvent message)
    {
        _logger.LogInformation(
            "Branch modified. Id: {Id}, Name: {Name}, Adress: {Address}, Status: {Status}, CreatedAt: {CreatedAt}, ModifiedAt: {UpdatedAt}",
            message.Id,
            message.Name,
            message.Address,
            message.Status,
            message.CreatedAt,
            message.UpdatedAt);

        return Task.CompletedTask;
    }
}


