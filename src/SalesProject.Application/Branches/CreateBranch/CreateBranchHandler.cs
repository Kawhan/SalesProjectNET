using AutoMapper;
using MediatR;
using SalesProject.Domain.Entities;
using SalesProject.Application.Branches.Events;
using SalesProject.Application.Common.Messaging;
using SalesProject.Domain.Repositories;
using FluentValidation;


namespace SalesProject.Application.Branches.CreateBranch;

/// <summary>
/// Handler for processing CreateBranchCommand requests.
/// </summary>
public class CreateBranchHandler : IRequestHandler<CreateBranchCommand, CreateBranchResult>
{
    private readonly IBranchRepository _branchRepository;
    private readonly IMapper _mapper;
    private readonly IMessageBusService _messageBusService;

    /// <summary>
    /// Initializes a new instance of CreateBranchHandler.
    /// </summary>
    /// <param name="branchRepository">The branch repository.</param>
    /// <param name="mapper">The AutoMapper instance.</param>
    /// <param name="messageBusService">The Message Bus Service instance</param>
    public CreateBranchHandler(
        IBranchRepository branchRepository,
        IMapper mapper,
        IMessageBusService messageBusService)
    {
        _branchRepository = branchRepository;
        _mapper = mapper;
        _messageBusService = messageBusService;
    }

    /// <summary>
    /// Handles the CreateBranchCommand request.
    /// </summary>
    /// <param name="request">The CreateBranch command.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The created branch details.</returns>
    public async Task<CreateBranchResult> Handle(
        CreateBranchCommand request,
        CancellationToken cancellationToken)
    {
        var validator = new CreateBranchCommandValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            throw new ValidationException(validationResult.Errors);

        var branch = _mapper.Map<Branch>(request);

        var createdBranch = await _branchRepository.CreateAsync(branch, cancellationToken);

        await _messageBusService.PublishAsync(new BranchCreatedEvent
        {
            Id = createdBranch.Id,
            Address = createdBranch.Address,
            Name = createdBranch.Name,
            Status = createdBranch.Status,
            CreatedAt = createdBranch.CreatedAt
        }, cancellationToken);

        return _mapper.Map<CreateBranchResult>(createdBranch);
    }
}