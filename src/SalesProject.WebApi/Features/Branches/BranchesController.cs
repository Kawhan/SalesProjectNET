using AutoMapper;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using SalesProject.Application.Branches.CreateBranch;
using SalesProject.Application.Branches.DeleteBranch;
using SalesProject.Application.Branches.GetAllBranches;
using SalesProject.Application.Branches.GetBranch;
using SalesProject.Application.Branches.UpdateBranch;
using SalesProject.WebApi.Common;
using SalesProject.WebApi.Features.Branches.CreateBranch;
using SalesProject.WebApi.Features.Branches.DeleteBranch;
using SalesProject.WebApi.Features.Branches.GetAllBranches;
using SalesProject.WebApi.Features.Branches.GetBranch;
using SalesProject.WebApi.Features.Branches.UpdateBranch;

namespace SalesProject.WebApi.Features.Branches;

[Route("api/[controller]")]
[ApiController]
public class BranchesController : BaseController
{
    private readonly IMediator _mediator;
    private readonly IMapper _mapper;

    /// <summary>
    /// Initializes a new instance of BranchesController
    /// </summary>
    /// <param name="mediator">The mediator instance</param>
    /// <param name="mapper">The AutoMapper instance</param>
    public BranchesController(IMediator mediator, IMapper mapper)
    {
        _mediator = mediator;
        _mapper = mapper;
    }

    /// <summary>
    /// Creates a new branch
    /// </summary>
    /// <param name="request">The branch creation request</param>
    /// <param name="cancellationToken">Cancellation token</param>
    /// <returns>The created branch details</returns>
    [HttpPost]
    [ProducesResponseType(typeof(ApiResponseWithData<CreateBranchResponse>), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> CreateBranch(
        [FromBody] CreateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new CreateBranchRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<CreateBranchCommand>(request);
        var response = await _mediator.Send(command, cancellationToken);

        return Created(string.Empty, new ApiResponseWithData<CreateBranchResponse>
        {
            Success = true,
            Message = "Branch created successfully",
            Data = _mapper.Map<CreateBranchResponse>(response)
        });
    }

    /// <summary>
    /// Retrieves a branch by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the branch.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The branch details if found.</returns>
    [HttpGet("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<GetBranchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> GetBranch(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new GetBranchRequest { Id = id };
        var validator = new GetBranchRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<GetBranchCommand>(request.Id);
        var response = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<GetBranchResponse>
        {
            Success = true,
            Message = "Branch retrieved successfully",
            Data = _mapper.Map<GetBranchResponse>(response)
        });
    }

    /// <summary>
    /// Retrieves branches with pagination and filters.
    /// </summary>
    /// <param name="request">The branch list request with pagination and filters.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The paginated list of branches.</returns>
    [HttpGet]
    [ProducesResponseType(typeof(PaginatedResponse<GetAllBranchesResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> GetAllBranches(
        [FromQuery] GetAllBranchesRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new GetAllBranchesRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<GetAllBranchesCommand>(request);
        var result = await _mediator.Send(command, cancellationToken);

        var branches = _mapper.Map<List<GetAllBranchesResponse>>(result.Data);

        var paginatedBranches = new PaginatedList<GetAllBranchesResponse>(
            branches,
            result.TotalCount,
            result.CurrentPage,
            result.PageSize
        );

        return OkPaginated(paginatedBranches);
    }

    /// <summary>
    /// Updates a branch by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the branch to update.</param>
    /// <param name="request">The branch update request.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>The updated branch details.</returns>
    [HttpPut("{id}")]
    [ProducesResponseType(typeof(ApiResponseWithData<UpdateBranchResponse>), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> UpdateBranch(
        [FromRoute] Guid id,
        [FromBody] UpdateBranchRequest request,
        CancellationToken cancellationToken)
    {
        var validator = new UpdateBranchRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<UpdateBranchCommand>(request);
        command.Id = id;

        var result = await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponseWithData<UpdateBranchResponse>
        {
            Success = true,
            Message = "Branch updated successfully",
            Data = _mapper.Map<UpdateBranchResponse>(result)
        });
    }

    /// <summary>
    /// Deletes a branch by its ID.
    /// </summary>
    /// <param name="id">The unique identifier of the branch to delete.</param>
    /// <param name="cancellationToken">Cancellation token.</param>
    /// <returns>Success response if the branch was deleted.</returns>
    [HttpDelete("{id}")]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status200OK)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ApiResponse), StatusCodes.Status404NotFound)]
    public async Task<IActionResult> DeleteBranch(
        [FromRoute] Guid id,
        CancellationToken cancellationToken)
    {
        var request = new DeleteBranchRequest { Id = id };
        var validator = new DeleteBranchRequestValidator();
        var validationResult = await validator.ValidateAsync(request, cancellationToken);

        if (!validationResult.IsValid)
            return BadRequest(validationResult.Errors);

        var command = _mapper.Map<DeleteBranchCommand>(request.Id);

        await _mediator.Send(command, cancellationToken);

        return Ok(new ApiResponse
        {
            Success = true,
            Message = "Branch deleted successfully"
        });
    }
}