using SalesProject.Domain.Entities;

namespace SalesProject.Domain.Events;

public class UserRegisteredEvent
{
    public User User { get; }

    public UserRegisteredEvent(User user)
    {
        User = user;
    }
}