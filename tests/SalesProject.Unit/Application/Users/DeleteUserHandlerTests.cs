using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Users.DeleteUser;
using SalesProject.Application.Users.Events;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Users.TestData;

namespace SalesProject.Unit.Application.Users;

/// <summary>
/// Contains unit tests for the <see cref="DeleteUserHandler"/> class.
/// </summary>
public class DeleteUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageBusService _messageBusService;
    private readonly DeleteUserHandler _handler;

    public DeleteUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _messageBusService = Substitute.For<IMessageBusService>();
        _handler = new DeleteUserHandler(_userRepository, _messageBusService);
    }

    /// <summary>
    /// Tests that a valid user delete request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid user id When deleting user Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = DeleteUserHandlerTestData.GenerateValidCommand();

        var user = DeleteUserHandlerTestData.GenerateUser();
        user.Id = command.Id;

        _userRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        // When
        var result = await _handler.Handle(command, CancellationToken.None);

        // Then
        result.Should().NotBeNull();
        result.Success.Should().BeTrue();

        await _userRepository.Received(1)
            .DeleteAsync(command.Id, Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid user delete request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid user id When deleting user Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new DeleteUserCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the user does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing user id When deleting user Then throws key not found exception")]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = DeleteUserHandlerTestData.GenerateValidCommand();

        _userRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(false);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User with ID {command.Id} not found");

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<UserDeletedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a UserDeletedEvent is published after deleting the user.
    /// </summary>
    [Fact(DisplayName = "Given valid user id When user is deleted Then publishes user deleted event")]
    public async Task Handle_ValidRequest_PublishesUserDeletedEvent()
    {
        // Given
        var command = DeleteUserHandlerTestData.GenerateValidCommand();

        var user = DeleteUserHandlerTestData.GenerateUser();
        user.Id = command.Id;

        _userRepository.DeleteAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(true);

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<UserDeletedEvent>(e =>
                e.Id == user.Id &&
                e.Username == user.Username &&
                e.Email == user.Email &&
                e.Phone == user.Phone &&
                e.Role == user.Role &&
                e.CreatedAt == user.CreatedAt &&
                e.UpdatedAt == user.UpdatedAt),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that no event is published when validation fails.
    /// </summary>
    [Fact(DisplayName = "Given invalid user id When deleting user Then does not publish user deleted event")]
    public async Task Handle_InvalidRequest_DoesNotPublishUserDeletedEvent()
    {
        // Given
        var command = new DeleteUserCommand(Guid.Empty);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<UserDeletedEvent>(), Arg.Any<CancellationToken>());
    }
}