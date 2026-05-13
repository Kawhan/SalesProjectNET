using Bogus;
using SalesProject.Application.Branches.UpdateBranch;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Branches.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases.
/// </summary>
public static class UpdateBranchHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid UpdateBranchCommand instances.
    /// </summary>
    private static readonly Faker<UpdateBranchCommand> updateBranchHandlerFaker = new Faker<UpdateBranchCommand>()
        .RuleFor(b => b.Id, f => f.Random.Guid())
        .RuleFor(b => b.Name, f => f.Company.CompanyName())
        .RuleFor(b => b.Address, f => f.Address.FullAddress())
        .RuleFor(b => b.Status, f => f.PickRandom(BranchStatus.Active, BranchStatus.Inactive));

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
    /// Generates a valid UpdateBranchCommand with randomized data.
    /// </summary>
    /// <returns>A valid UpdateBranchCommand with randomly generated data.</returns>
    public static UpdateBranchCommand GenerateValidCommand()
    {
        return updateBranchHandlerFaker.Generate();
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
    /// Generates an UpdateBranchResult object based on a Branch entity.
    /// </summary>
    /// <param name="branch">The branch entity.</param>
    /// <returns>An UpdateBranchResult object.</returns>
    public static UpdateBranchResult GenerateBranchResult(Branch branch)
    {
        return new UpdateBranchResult
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