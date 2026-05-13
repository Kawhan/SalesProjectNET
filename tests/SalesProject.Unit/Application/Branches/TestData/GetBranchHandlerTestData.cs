using Bogus;
using SalesProject.Application.Branches.GetBranch;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Branches.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases.
/// </summary>
public static class GetBranchHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid GetBranchCommand instances.
    /// </summary>
    private static readonly Faker<GetBranchCommand> getBranchHandlerFaker = new Faker<GetBranchCommand>()
        .CustomInstantiator(f => new GetBranchCommand(f.Random.Guid()));

    /// <summary>
    /// Configures the Faker to generate valid Branch entities.
    /// </summary>
    private static readonly Faker<Branch> branchFaker = new Faker<Branch>()
        .RuleFor(b => b.Id, f => f.Random.Guid())
        .RuleFor(b => b.Name, f => f.Company.CompanyName())
        .RuleFor(b => b.Address, f => f.Address.FullAddress())
        .RuleFor(b => b.Status, f => f.PickRandom(BranchStatus.Active, BranchStatus.Inactive))
        .RuleFor(b => b.CreatedAt, f => f.Date.Past())
        .RuleFor(b => b.UpdatedAt, f => f.Date.Recent());

    /// <summary>
    /// Generates a valid GetBranchCommand with randomized data.
    /// </summary>
    /// <returns>A valid GetBranchCommand with randomly generated data.</returns>
    public static GetBranchCommand GenerateValidCommand()
    {
        return getBranchHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates a valid Branch entity.
    /// </summary>
    /// <returns>A valid Branch entity with randomly generated data.</returns>
    public static Branch GenerateBranch()
    {
        return branchFaker.Generate();
    }

    /// <summary>
    /// Generates a GetBranchResult object based on a Branch entity.
    /// </summary>
    /// <param name="branch">The branch entity.</param>
    /// <returns>A GetBranchResult object.</returns>
    public static GetBranchResult GenerateBranchResult(Branch branch)
    {
        return new GetBranchResult
        {
            Id = branch.Id,
            Name = branch.Name,
            Address = branch.Address,
            Status = branch.Status,
            CreatedAt = branch.CreatedAt,
            UpdatedAt = branch.UpdatedAt
        };
    }
}