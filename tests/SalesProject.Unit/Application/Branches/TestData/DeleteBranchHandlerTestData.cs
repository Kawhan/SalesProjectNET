using Bogus;
using SalesProject.Application.Branches.DeleteBranch;

namespace SalesProject.Unit.Application.Branches.TestData;

/// <summary>
/// Provides methods for generating test data using the Bogus library.
/// This class centralizes all test data generation to ensure consistency
/// across test cases and provide both valid and invalid data scenarios.
/// </summary>
public static class DeleteBranchHandlerTestData
{
    /// <summary>
    /// Configures the Faker to generate valid DeleteBranchCommand instances.
    /// </summary>
    private static readonly Faker<DeleteBranchCommand> deleteBranchHandlerFaker = new Faker<DeleteBranchCommand>()
        .CustomInstantiator(f => new DeleteBranchCommand(f.Random.Guid()));

    /// <summary>
    /// Generates a valid DeleteBranchCommand with randomized data.
    /// </summary>
    /// <returns>A valid DeleteBranchCommand with randomly generated data.</returns>
    public static DeleteBranchCommand GenerateValidCommand()
    {
        return deleteBranchHandlerFaker.Generate();
    }
}