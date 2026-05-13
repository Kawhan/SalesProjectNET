using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Users.Events;

namespace SalesProject.IoC.Messaging.Handlers.Users;

public class UserDeletedEventHandler : IHandleMessages<UserDeletedEvent>
{
    private readonly ILogger<UserDeletedEventHandler> _logger;

    public UserDeletedEventHandler(ILogger<UserDeletedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserDeletedEvent message)
    {
        _logger.LogInformation(
            "User deleted. Id: {Id}, Username: {Username}, Email: {Email}, Phone: {Phone}, Role: {Role}, CreatedAt: {CreatedAt}, UpdatedAt: {UpdatedAt}",
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

