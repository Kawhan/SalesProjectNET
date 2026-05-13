using MediatR;
using SalesProject.Common.Validation;
using SalesProject.Domain.Enums;

namespace SalesProject.Application.Branches.CreateBranch;

/// <summary>
/// Command for creating a new branch.
/// </summary>
/// <remarks>
/// This command is used to capture the required data for creating a branch,
/// including name, address and status.
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request
/// that returns a <see cref="CreateBranchResult"/>.
/// 
/// The data provided in this command is validated using the
/// <see cref="CreateBranchCommandValidator"/> which extends
/// <see cref="FluentValidation.AbstractValidator{T}"/> to ensure that the fields are correctly
/// populated and follow the required rules.
/// </remarks>
public class CreateBranchCommand : IRequest<CreateBranchResult>
{
    /// <summary>
    /// Gets or sets the branch name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the branch address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the initial status of the branch.
    /// </summary>
    public BranchStatus Status { get; set; }

    /// <summary>
    /// Validates the current command using the CreateBranchCommandValidator.
    /// </summary>
    /// <returns>The validation result details.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new CreateBranchCommandValidator();
        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}
