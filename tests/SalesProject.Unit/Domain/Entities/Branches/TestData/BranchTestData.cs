using Bogus;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Domain.Entities.Branches.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// </summary>
public static class BranchTestData
{
    /// <summary>
    /// Configures the Faker to generate valid Branch entities.
    /// </summary>
    private static readonly Faker<Branch> BranchFaker = new Faker<Branch>()
        .RuleFor(b => b.Name, f => f.Company.CompanyName())
        .RuleFor(b => b.Address, f => f.Address.FullAddress())
        .RuleFor(b => b.Status, f => f.PickRandom(BranchStatus.Active, BranchStatus.Inactive));

    /// <summary>
    /// Generates a valid Branch entity with randomized data.
    /// </summary>
    /// <returns>A valid Branch entity with randomly generated data.</returns>
    public static Branch GenerateValidBranch()
    {
        return BranchFaker.Generate();
    }

    /// <summary>
    /// Generates a valid branch name.
    /// </summary>
    /// <returns>A valid branch name.</returns>
    public static string GenerateValidName()
    {
        return new Faker().Company.CompanyName();
    }

    /// <summary>
    /// Generates a valid branch address.
    /// </summary>
    /// <returns>A valid branch address.</returns>
    public static string GenerateValidAddress()
    {
        return new Faker().Address.FullAddress();
    }
}
