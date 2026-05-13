using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Domain.Specifications;

public class ActiveUserSpecification : ISpecification<User>
{
    public bool IsSatisfiedBy(User user)
    {
        return user.Status == UserStatus.Active;
    }
}
