using Bogus;
using SalesProject.Application.Branches.CreateBranch;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Branches.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class CreateBranchHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid CreateBranchCommand instances.
    /// The generated branches will have valid:
    /// - Name
    /// - Address
    /// - Status
    /// </summary>
    private static readonly Faker<CreateBranchCommand> createBranchHandlerFaker = new Faker<CreateBranchCommand>()
        .RuleFor(b => b.Name, f => f.Company.CompanyName())
        .RuleFor(b => b.Address, f => f.Address.FullAddress())
        .RuleFor(b => b.Status, f => f.PickRandom(BranchStatus.Active, BranchStatus.Inactive));

    /// <summary>
    /// Generates a valid CreateBranchCommand with randomized data.
    /// The generated command will have all properties populated with valid values
    /// that meet the system's validation requirements.
    /// </summary>
    /// <returns>A valid CreateBranchCommand with randomly generated data.</returns>
    public static CreateBranchCommand GenerateValidCommand()
    {
        return createBranchHandlerFaker.Generate();
    }
}