using MediatR;
using SalesProject.Common.Validation;
using SalesProject.Domain.Enums;

namespace SalesProject.Application.Branches.UpdateBranch;

/// <summary>
/// Command for updating an existing branch.
/// </summary>
/// <remarks>
/// This command is used to capture the required data for updating a branch,
/// including the branch identifier, name, address and status.
/// It implements <see cref="IRequest{TResponse}"/> to initiate the request
/// that returns a <see cref="UpdateBranchResult"/>.
/// 
/// The data provided in this command is validated using the
/// <see cref="UpdateBranchCommandValidator"/> which extends
/// <see cref="FluentValidation.AbstractValidator{T}"/> to ensure that the fields are correctly
/// populated and follow the required rules.
/// </remarks>
public class UpdateBranchCommand : IRequest<UpdateBranchResult>
{
    /// <summary>
    /// Gets or sets the unique identifier of the branch to update.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the updated branch name.
    /// </summary>
    public string Name { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the updated branch address.
    /// </summary>
    public string? Address { get; set; }

    /// <summary>
    /// Gets or sets the updated branch status.
    /// </summary>
    public BranchStatus Status { get; set; }

    /// <summary>
    /// Validates the current command using the UpdateBranchCommandValidator.
    /// </summary>
    /// <returns>The validation result details.</returns>
    public ValidationResultDetail Validate()
    {
        var validator = new UpdateBranchCommandValidator();
        var result = validator.Validate(this);

        return new ValidationResultDetail
        {
            IsValid = result.IsValid,
            Errors = result.Errors.Select(o => (ValidationErrorDetail)o)
        };
    }
}