using SalesProject.Domain.Entities;
using SalesProject.Domain.Enums;
using SalesProject.Unit.Domain.Entities.Branches.TestData;

namespace SalesProject.Unit.Domain.Entities.Branches;

/// <summary>
/// Contains unit tests for the Branch entity class.
/// Tests cover status changes and update scenarios.
/// </summary>
public class BranchTests
{
    /// <summary>
    /// Tests that a new branch has CreatedAt set when instantiated.
    /// </summary>
    [Fact(DisplayName = "Branch should have CreatedAt set when created")]
    public void Given_NewBranch_When_Created_Then_CreatedAtShouldBeSet()
    {
        // Arrange & Act
        var branch = new Branch();

        // Assert
        Assert.NotEqual(default, branch.CreatedAt);
    }

    /// <summary>
    /// Tests that when an inactive branch is activated, its status changes to Active.
    /// </summary>
    [Fact(DisplayName = "Branch status should change to Active when activated")]
    public void Given_InactiveBranch_When_Activated_Then_StatusShouldBeActive()
    {
        // Arrange
        var branch = BranchTestData.GenerateValidBranch();
        branch.Status = BranchStatus.Inactive;

        // Act
        branch.Activate();

        // Assert
        Assert.Equal(BranchStatus.Active, branch.Status);
        Assert.NotNull(branch.UpdatedAt);
    }

    /// <summary>
    /// Tests that when an active branch is deactivated, its status changes to Inactive.
    /// </summary>
    [Fact(DisplayName = "Branch status should change to Inactive when deactivated")]
    public void Given_ActiveBranch_When_Deactivated_Then_StatusShouldBeInactive()
    {
        // Arrange
        var branch = BranchTestData.GenerateValidBranch();
        branch.Status = BranchStatus.Active;

        // Act
        branch.Deactivate();

        // Assert
        Assert.Equal(BranchStatus.Inactive, branch.Status);
        Assert.NotNull(branch.UpdatedAt);
    }

    /// <summary>
    /// Tests that branch information is updated correctly.
    /// </summary>
    [Fact(DisplayName = "Branch data should be updated when update is called")]
    public void Given_Branch_When_Updated_Then_DataShouldBeChanged()
    {
        // Arrange
        var branch = BranchTestData.GenerateValidBranch();
        var newName = BranchTestData.GenerateValidName();
        var newAddress = BranchTestData.GenerateValidAddress();
        var newStatus = BranchStatus.Active;

        // Act
        branch.Update(newName, newAddress, newStatus);

        // Assert
        Assert.Equal(newName, branch.Name);
        Assert.Equal(newAddress, branch.Address);
        Assert.Equal(newStatus, branch.Status);
        Assert.NotNull(branch.UpdatedAt);
    }

    /// <summary>
    /// Tests that branch address can be updated to null.
    /// </summary>
    [Fact(DisplayName = "Branch address should allow null when updated")]
    public void Given_Branch_When_UpdatedWithNullAddress_Then_AddressShouldBeNull()
    {
        // Arrange
        var branch = BranchTestData.GenerateValidBranch();
        var newName = BranchTestData.GenerateValidName();
        var newStatus = BranchStatus.Active;

        // Act
        branch.Update(newName, null, newStatus);

        // Assert
        Assert.Equal(newName, branch.Name);
        Assert.Null(branch.Address);
        Assert.Equal(newStatus, branch.Status);
        Assert.NotNull(branch.UpdatedAt);
    }
}