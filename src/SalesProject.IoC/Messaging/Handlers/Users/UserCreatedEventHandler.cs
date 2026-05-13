using Microsoft.Extensions.Logging;
using Rebus.Handlers;
using SalesProject.Application.Users.Events;

namespace SalesProject.IoC.Messaging.Handlers.Users;

public class UserCreatedEventHandler : IHandleMessages<UserCreatedEvent>
{
    private readonly ILogger<UserCreatedEventHandler> _logger;

    public UserCreatedEventHandler(ILogger<UserCreatedEventHandler> logger)
    {
        _logger = logger;
    }

    public Task Handle(UserCreatedEvent message)
    {
        _logger.LogInformation(
            "User created. Id: {Id}, Username: {Username}, Email: {Email}, Phone: {Phone}, Role: {Role}, CreatedAt: {CreatedAt}",
            message.Id,
            message.Username,
            message.Email,
            message.Phone,
            message.Role,
            message.CreatedAt);

        return Task.CompletedTask;
    }
}

