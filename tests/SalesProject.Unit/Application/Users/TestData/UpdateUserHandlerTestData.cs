using Bogus;
using SalesProject.Application.Users.UpdateUser;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Users.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// </summary>
public static class UpdateUserHandlerTestData
{
    private static readonly Faker<UpdateUserCommand> updateUserHandlerFaker = new Faker<UpdateUserCommand>()
        .RuleFor(u => u.Id, f => f.Random.Guid())
        .RuleFor(u => u.Username, f => f.Internet.UserName())
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
        .RuleFor(u => u.Status, f => f.PickRandom(UserStatus.Active, UserStatus.Suspended))
        .RuleFor(u => u.Role, f => f.PickRandom(UserRole.Customer, UserRole.Admin));

    private static readonly Faker<User> userFaker = new Faker<User>()
        .RuleFor(u => u.Id, f => f.Random.Guid())
        .RuleFor(u => u.Username, f => f.Internet.UserName())
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
        .RuleFor(u => u.Password, f => $"Test@{f.Random.Number(100, 999)}")
        .RuleFor(u => u.Status, f => f.PickRandom(UserStatus.Active, UserStatus.Suspended))
        .RuleFor(u => u.Role, f => f.PickRandom(UserRole.Customer, UserRole.Admin))
        .RuleFor(u => u.CreatedAt, f => f.Date.Past())
        .RuleFor(u => u.UpdatedAt, f => f.Date.Recent());

    public static UpdateUserCommand GenerateValidCommand()
    {
        return updateUserHandlerFaker.Generate();
    }

    public static User GenerateUser(Guid? id = null)
    {
        var user = userFaker.Generate();

        if (id.HasValue)
            user.Id = id.Value;

        return user;
    }

    public static UpdateUserResult GenerateResult(User user)
    {
        return new UpdateUserResult
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Phone = user.Phone,
            Role = user.Role,
            Status = user.Status,
            CreatedAt = user.CreatedAt,
            UpdatedAt = user.UpdatedAt
        };
    }
}