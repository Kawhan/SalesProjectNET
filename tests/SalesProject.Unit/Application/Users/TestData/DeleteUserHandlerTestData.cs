using Bogus;
using SalesProject.Application.Users.DeleteUser;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Users.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// </summary>
public static class DeleteUserHandlerTestData
{
    private static readonly Faker<DeleteUserCommand> deleteUserHandlerFaker = new Faker<DeleteUserCommand>()
        .CustomInstantiator(f => new DeleteUserCommand(f.Random.Guid()));

    private static readonly Faker<User> userFaker = new Faker<User>()
        .RuleFor(u => u.Id, f => f.Random.Guid())
        .RuleFor(u => u.Username, f => f.Internet.UserName())
        .RuleFor(u => u.Email, f => f.Internet.Email())
        .RuleFor(u => u.Phone, f => $"+55{f.Random.Number(11, 99)}{f.Random.Number(100000000, 999999999)}")
        .RuleFor(u => u.Password, f => $"Test@{f.Random.Number(100, 999)}")
        .RuleFor(u => u.Status, UserStatus.Inactive)
        .RuleFor(u => u.Role, f => f.PickRandom(UserRole.Customer, UserRole.Admin))
        .RuleFor(u => u.CreatedAt, f => f.Date.Past())
        .RuleFor(u => u.UpdatedAt, f => f.Date.Recent());

    public static DeleteUserCommand GenerateValidCommand()
    {
        return deleteUserHandlerFaker.Generate();
    }

    public static User GenerateUser()
    {
        return userFaker.Generate();
    }
}