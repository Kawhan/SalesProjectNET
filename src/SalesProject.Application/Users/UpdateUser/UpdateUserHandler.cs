using AutoMapper;
using MediatR;
using SalesProject.Application.Common.Messaging;
using SalesProject.Application.Users.Events;
using SalesProject.Domain.Repositories;
using FluentValidation;

namespace SalesProject.Application.Users.UpdateUser;

/// <summary>
/// Handler for processing UpdateUserCommand requests.
/// </summary>
public class UpdateUserHandler : IRequestHandler<UpdateUserCommand, UpdateUserResult>
{
    private readonly IUserRepository _userRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;

    /// <summary>
    /// Initializes a new instance of UpdateUserHandler.
    /// </summary>
    /// <param name="userRepository">The user repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="messageBusService">The Message Bus Service instance</param>
    public UpdateUserHandler(
        IUserRepository userRepository,
        IMapper mapper, IMessageBusService messageBusService)
    {
        _userRepository = userRepository;
        _mapper = mapper;
        _messageBusService = messageBusService;
    }

    /// <summary>
    /// Handles the UpdateUserCommand request.
    /// </summary>
    /// <param name="request">The UpdateUser command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated user details.</returns>
    public async Task<UpdateUserResult> Handle(
        UpdateUserCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new UpdateUserCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var user = await _userRepository.GetByIdAsync(request.Id, cancellationToken);

        if (user is null)
            throw new KeyNotFoundException($"User with id {request.Id} was not found.");

        user.Update(
            request.Username,
            request.Email,
            request.Phone,
            request.Status,
            request.Role
        );

        var updatedUser = await _userRepository.UpdateAsync(user, cancellationToken);

        await _messageBusService.PublishAsync(new UserModifiedEvent
        {
            Id = updatedUser.Id,
            Username = updatedUser.Username,
            Email = updatedUser.Email,
            Phone = updatedUser.Phone,
            Role = updatedUser.Role,
            CreatedAt = updatedUser.CreatedAt,
            UpdatedAt = updatedUser.UpdatedAt
        }, cancellationToken);

        return _mapper.Map<UpdateUserResult>(updatedUser);
    }
}