using MediatR;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Users.Events;
using SalesProject.Domain.Repositories;

using FluentValidation;

namespace SalesProject.Application.Users.DeleteUser;

/// <summary>
/// Handler for processing DeleteUserCommand requests
/// </summary>
public class DeleteUserHandler : IRequestHandler<DeleteUserCommand, DeleteUserResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly IMessageBusService _messageBusService;


    /// <summary>
    /// Initializes a new instance of DeleteUserHandler
    /// </summary>
    /// <param name="userRepository">The user repository</param>
    /// <param name="messageBusService">The Message Bus Service instance</param>
    public DeleteUserHandler(
        IUserRepository userRepository, IMessageBusService messageBusService)
    {
        _userRepository = userRepository;
        _messageBusService = messageBusService;
    }

    /// <summary>
    /// Handles the DeleteUserCommand request
    /// </summary>
    /// <param name="request">The DeleteUser command</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The result of the delete operation</returns>
    public async Task<DeleteUserResponse> Handle(DeleteUserCommand request, CancellationToken cancellationToken)
    {
        var validator = new DeleteUserValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var success = await _userRepository.DeleteAsync(request.Id, cancellationToken);
        if (!success)
            throw new KeyNotFoundException($"User with ID {request.Id} not found");

        var deletedUser = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (deletedUser == null)
            throw new KeyNotFoundException($"User with ID {request.Id} not found");

        await _messageBusService.PublishAsync(new UserDeletedEvent
        {
            Id = deletedUser.Id,
            Username = deletedUser.Username,
            Email = deletedUser.Email,
            Phone = deletedUser.Phone,
            Role = deletedUser.Role,
            CreatedAt = deletedUser.CreatedAt,
            UpdatedAt = deletedUser.UpdatedAt
        }, cancellationToken);

        return new DeleteUserResponse { Success = true };
    }
}
