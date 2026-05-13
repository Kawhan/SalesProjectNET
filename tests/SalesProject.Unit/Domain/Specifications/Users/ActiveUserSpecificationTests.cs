using SalesProject.Domain.Enums;
using SalesProject.Domain.Specifications;
using SalesProject.Unit.Domain.Specifications.Users.TestData;
using FluentAssertions;

namespace SalesProject.Unit.Domain.Specifications.Users;

public class ActiveUserSpecificationTests
{
    [Theory]
    [InlineData(UserStatus.Active, true)]
    [InlineData(UserStatus.Inactive, false)]
    [InlineData(UserStatus.Suspended, false)]
    public void IsSatisfiedBy_ShouldValidateUserStatus(UserStatus status, bool expectedResult)
    {
        // Arrange
        var user = ActiveUserSpecificationTestData.GenerateUser(status);
        var specification = new ActiveUserSpecification();

        // Act
        var result = specification.IsSatisfiedBy(user);

        // Assert
        result.Should().Be(expectedResult);
    }
}