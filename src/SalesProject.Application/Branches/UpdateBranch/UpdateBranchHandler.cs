using AutoMapper;
using MediatR;
using SalesProject.Application.Branches.Events;
using SalesProject.Application.Common.Messaging;
using SalesProject.Domain.Repositories;

using FluentValidation;

namespace SalesProject.Application.Branches.UpdateBranch;

/// <summary>
/// Handler for processing UpdateBranchCommand requests.
/// </summary>
public class UpdateBranchHandler : IRequestHandler<UpdateBranchCommand, UpdateBranchResult>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;

    /// <summary>
    /// Initializes a new instance of UpdateBranchHandler.
    /// </summary>
    /// <param name="branchRepository">The branch repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="messageBusService">The Message Bus Service instance</param>
    public UpdateBranchHandler(
        IBranchRepository branchRepository,
        IMapper mapper,
        IMessageBusService messageBusService)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
        _messageBusService = messageBusService;
    }

    /// <summary>
    /// Handles the UpdateBranchCommand request.
    /// </summary>
    /// <param name="request">The UpdateBranch command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated branch details.</returns>
    public async Task<UpdateBranchResult> Handle(
        UpdateBranchCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new UpdateBranchCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var branch = await _branchRepository.GetByIdAsync(request.Id, cancellationToken);

        if (branch is null)
            throw new KeyNotFoundException($"Branch with id {request.Id} was not found.");

        branch.Update(
            request.Name,
            request.Address,
            request.Status
        );

        var updatedBranch = await _branchRepository.UpdateAsync(branch, cancellationToken);

        await _messageBusService.PublishAsync(new BranchModifiedEvent
        {
            Id = updatedBranch.Id,
            Address = updatedBranch.Address,
            Name = updatedBranch.Name,
            Status = updatedBranch.Status,
            CreatedAt = updatedBranch.CreatedAt,
            UpdatedAt = updatedBranch.UpdatedAt.GetValueOrDefault(DateTime.Now)
        }, cancellationToken);

        return _mapper.Map<UpdateBranchResult>(updatedBranch);
    }
}