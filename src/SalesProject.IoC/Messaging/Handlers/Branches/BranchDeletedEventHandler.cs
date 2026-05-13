using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Branches.Events;


namespace SalesProject.IoC.Messaging.Handlers.Branches;

/// <summary>
/// Handles BranchDeletedEvent messages.
/// </summary>
public class BranchDeletedEventHandler : IHandleMessages<BranchDeletedEvent>
{
    private readonly ILogger<BranchDeletedEventHandler> _logger;

    public BranchDeletedEventHandler(ILogger<BranchDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(BranchDeletedEvent message)
    {
        _logger.LogInformation(
            "Branch deleted. Id: {Id}, Name: {Name}, Adress: {Address}, Status: {Status}, CreatedAt: {CreatedAt}, DeletedAt: {UpdatedAt}",
            message.Id,
            message.Name,
            message.Address,
            message.Status,
            message.CreatedAt,
            message.UpdatedAt);

        return Task.CompletedTask;
    }
}

