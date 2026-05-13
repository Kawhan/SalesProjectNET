using Bogus;
using SalesProject.Application.Branches.GetAllBranches;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;

namespace SalesProject.Unit.Application.Branches.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases.
/// </summary>
public static class GetAllBranchesHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid GetAllBranchesCommand instances.
    /// </summary>
    private static readonly Faker<GetAllBranchesCommand> getAllBranchesHandlerFaker = new Faker<GetAllBranchesCommand>()
        .RuleFor(b => b.PageNumber, f => f.Random.Int(1, 5))
        .RuleFor(b => b.PageSize, f => f.Random.Int(5, 20))
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
    /// Generates a valid GetAllBranchesCommand with randomized data.
    /// </summary>
    /// <returns>A valid GetAllBranchesCommand with randomly generated data.</returns>
    public static GetAllBranchesCommand GenerateValidCommand()
    {
        return getAllBranchesHandlerFaker.Generate();
    }

    /// <summary>
    /// Generates a list of valid Branch entities.
    /// </summary>
    /// <param name="count">The number of branches to generate.</param>
    /// <returns>A list of valid Branch entities.</returns>
    public static List<Branch> GenerateBranches(int count)
    {
        return branchFaker.Generate(count);
    }

    /// <summary>
    /// Generates a list of GetAllBranchesResult objects based on branch entities.
    /// </summary>
    /// <param name="branches">The branch entities.</param>
    /// <returns>A list of GetAllBranchesResult objects.</returns>
    public static List<GetAllBranchesResult> GenerateBranchResults(List<Branch> branches)
    {
        return branches.Select(branch => new GetAllBranchesResult
        {
            Id = branch.Id,
            Name = branch.Name,
            Address = branch.Address,
            Status = branch.Status,
            CreatedAt = branch.CreatedAt,
            UpdatedAt = branch.UpdatedAt
        }).ToList();
    }
}