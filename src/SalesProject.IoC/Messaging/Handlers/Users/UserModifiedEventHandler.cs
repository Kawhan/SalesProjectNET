using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Users.Events;

namespace SalesProject.IoC.Messaging.Handlers.Users;

public class UserModifiedEventHandler : IHandleMessages<UserModifiedEvent>
{
    private readonly ILogger<UserModifiedEventHandler> _logger;

    public UserModifiedEventHandler(ILogger<UserModifiedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserModifiedEvent message)
    {
        _logger.LogInformation(
            "User modified. Id: {Id}, Username: {Username}, Email: {Email}, Phone: {Phone}, Role: {Role}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}",
            message.Id,
            message.Username,
            message.Email,
            message.Phone,
            message.Role,
            message.CreatedAt,
            message.UpdatedAt);

        return Task.CompletedTask;
    }
}


