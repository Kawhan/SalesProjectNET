using AutoMapper;
using FluentAssertions;
using NSubstitute;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Users.Events;
using SalesProject.Application.Users.UpdateUser;
using SalesProject.Domain.Entities;
using SalesProject.Domain.Repositories;
using SalesProject.Unit.Application.Users.TestData;

namespace SalesProject.Unit.Application.Users;

/// <summary>
/// Contains unit tests for the <see cref="UpdateUserHandler"/> class.
/// </summary>
public class UpdateUserHandlerTests
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;
    private readonly UpdateUserHandler _handler;

    public UpdateUserHandlerTests()
    {
        _userRepository = Substitute.For<IUserRepository>();
        _mapper = Substitute.For<IMapper>();
        _messageBusService = Substitute.For<IMessageBusService>();

        _handler = new UpdateUserHandler(
            _userRepository,
            _mapper,
            _messageBusService);
    }

    /// <summary>
    /// Tests that a valid user update request is handled successfully.
    /// </summary>
    [Fact(DisplayName = "Given valid user data When updating user Then returns success response")]
    public async Task Handle_ValidRequest_ReturnsSuccessResponse()
    {
        // Given
        var command = UpdateUserHandlerTestData.GenerateValidCommand();

        var user = UpdateUserHandlerTestData.GenerateUser(command.Id);
        var result = UpdateUserHandlerTestData.GenerateResult(user);

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        _userRepository.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        _mapper.Map<UpdateUserResult>(Arg.Any<User>())
            .Returns(result);

        // When
        var updateUserResult = await _handler.Handle(command, CancellationToken.None);

        // Then
        updateUserResult.Should().NotBeNull();
        updateUserResult.Id.Should().Be(command.Id);

        await _userRepository.Received(1)
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an invalid user update request throws a validation exception.
    /// </summary>
    [Fact(DisplayName = "Given invalid user data When updating user Then throws validation exception")]
    public async Task Handle_InvalidRequest_ThrowsValidationException()
    {
        // Given
        var command = new UpdateUserCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();
    }

    /// <summary>
    /// Tests that when validation fails, no dependency is called.
    /// </summary>
    [Fact(DisplayName = "Given invalid user data When updating user Then does not call dependencies")]
    public async Task Handle_InvalidRequest_DoesNotCallDependencies()
    {
        // Given
        var command = new UpdateUserCommand();

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should().ThrowAsync<FluentValidation.ValidationException>();

        await _userRepository.DidNotReceive()
            .GetByIdAsync(Arg.Any<Guid>(), Arg.Any<CancellationToken>());

        await _userRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<UserModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a KeyNotFoundException is thrown when the user does not exist.
    /// </summary>
    [Fact(DisplayName = "Given non-existing user When updating user Then throws key not found exception")]
    public async Task Handle_UserNotFound_ThrowsKeyNotFoundException()
    {
        // Given
        var command = UpdateUserHandlerTestData.GenerateValidCommand();

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns((User?)null);

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<KeyNotFoundException>()
            .WithMessage($"User with id {command.Id} was not found.");

        await _userRepository.DidNotReceive()
            .UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>());

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<UserModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that the user is updated with command data.
    /// </summary>
    [Fact(DisplayName = "Given valid user data When updating user Then updates user with command data")]
    public async Task Handle_ValidRequest_UpdatesUserWithCommandData()
    {
        // Given
        var command = UpdateUserHandlerTestData.GenerateValidCommand();

        var user = UpdateUserHandlerTestData.GenerateUser(command.Id);
        var result = UpdateUserHandlerTestData.GenerateResult(user);

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        _userRepository.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        _mapper.Map<UpdateUserResult>(Arg.Any<User>())
            .Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _userRepository.Received(1).UpdateAsync(
            Arg.Is<User>(u =>
                u.Id == command.Id &&
                u.Username == command.Username &&
                u.Email == command.Email &&
                u.Phone == command.Phone &&
                u.Status == command.Status &&
                u.Role == command.Role &&
                u.UpdatedAt.HasValue),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that a UserModifiedEvent is published after updating the user.
    /// </summary>
    [Fact(DisplayName = "Given valid user data When user is updated Then publishes user modified event")]
    public async Task Handle_ValidRequest_PublishesUserModifiedEvent()
    {
        // Given
        var command = UpdateUserHandlerTestData.GenerateValidCommand();

        var user = UpdateUserHandlerTestData.GenerateUser(command.Id);
        var result = UpdateUserHandlerTestData.GenerateResult(user);

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        _userRepository.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        _mapper.Map<UpdateUserResult>(Arg.Any<User>())
            .Returns(result);

        // When
        await _handler.Handle(command, CancellationToken.None);

        // Then
        await _messageBusService.Received(1).PublishAsync(
            Arg.Is<UserModifiedEvent>(e =>
                e.Id == user.Id &&
                e.Username == command.Username &&
                e.Email == command.Email &&
                e.Phone == command.Phone &&
                e.Role == command.Role &&
                e.CreatedAt == user.CreatedAt &&
                e.UpdatedAt == user.UpdatedAt),
            Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that no event is published when repository update fails.
    /// </summary>
    [Fact(DisplayName = "Given valid user data When repository fails Then does not publish user modified event")]
    public async Task Handle_ValidRequest_WhenRepositoryFails_DoesNotPublishUserModifiedEvent()
    {
        // Given
        var command = UpdateUserHandlerTestData.GenerateValidCommand();
        var user = UpdateUserHandlerTestData.GenerateUser(command.Id);

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        _userRepository.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns<Task<User>>(_ => throw new InvalidOperationException("Repository error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Repository error");

        await _messageBusService.DidNotReceive()
            .PublishAsync(Arg.Any<UserModifiedEvent>(), Arg.Any<CancellationToken>());
    }

    /// <summary>
    /// Tests that an exception is thrown when the message bus fails.
    /// </summary>
    [Fact(DisplayName = "Given updated user When message bus fails Then throws exception")]
    public async Task Handle_ValidRequest_WhenMessageBusFails_ThrowsException()
    {
        // Given
        var command = UpdateUserHandlerTestData.GenerateValidCommand();
        var user = UpdateUserHandlerTestData.GenerateUser(command.Id);

        _userRepository.GetByIdAsync(command.Id, Arg.Any<CancellationToken>())
            .Returns(user);

        _userRepository.UpdateAsync(Arg.Any<User>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => callInfo.Arg<User>());

        _messageBusService
            .PublishAsync(Arg.Any<UserModifiedEvent>(), Arg.Any<CancellationToken>())
            .Returns<Task>(_ => throw new InvalidOperationException("Message bus error"));

        // When
        var act = () => _handler.Handle(command, CancellationToken.None);

        // Then
        await act.Should()
            .ThrowAsync<InvalidOperationException>()
            .WithMessage("Message bus error");

        _mapper.DidNotReceive()
            .Map<UpdateUserResult>(Arg.Any<User>());
    }
}