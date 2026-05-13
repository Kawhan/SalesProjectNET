using MediatR;
using SalesProject.Application.Branches.Events;
using SalesProject.Application.Common.Messaging;
using SalesProject.Domain.Repositories;
using FluentValidation;

namespace SalesProject.Application.Branches.DeleteBranch;

/// <summary>
/// Handler for processing DeleteBranchCommand requests.
/// </summary>
public class DeleteBranchHandler : IRequestHandler<DeleteBranchCommand, DeleteBranchResponse>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMessageBusService _messageBusService;

    /// <summary>
    /// Initializes a new instance of DeleteBranchHandler.
    /// </summary>
    /// <param name="branchRepository">The branch repository.</param>
    public DeleteBranchHandler(IBranchRepository branchRepository, IMessageBusService messageBusService)
    {
        _branchRepository = branchRepository;
        _messageBusService = messageBusService;
    }

    /// <summary>
    /// Handles the DeleteBranchCommand request.
    /// </summary>
    /// <param name="request">The DeleteBranch command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The result of the delete operation.</returns>
    public async Task<DeleteBranchResponse> Handle(
        DeleteBranchCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new DeleteBranchValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var success = await _branchRepository.DeleteAsync(request.Id, cancellationToken);

        if (!success)
            throw new KeyNotFoundException($"Branch with ID {request.Id} not found.");

        var deletedBranch = await _branchRepository.GetByIdAsync(request.Id);

        await _messageBusService.PublishAsync(new BranchDeletedEvent
        {
            Id = deletedBranch.Id,
            Address = deletedBranch.Address,
            Name = deletedBranch.Name,
            Status = deletedBranch.Status,
            CreatedAt = deletedBranch.CreatedAt,
            UpdatedAt = deletedBranch.UpdatedAt.GetValueOrDefault(DateTime.Now)
        }, cancellationToken);

        return new DeleteBranchResponse
        {
            Success = true
        };
    }
}