using FluentValidation;

namespace SalesProject.WebApi.Features.Sales.GetAllSales;

/// <summary>
/// Validator for GetAllSalesRequest.
/// </summary>
public class GetAllSalesRequestValidator : AbstractValidator<GetAllSalesRequest>
{
    /// <summary>
    /// Initializes a new instance of the GetAllSalesRequestValidator with defined validation rules.
    /// </summary>
    public GetAllSalesRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");

        RuleFor(request => request.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than zero.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be less than or equal to 100.");

        RuleFor(request => request.MinTotalAmount)
            .GreaterThanOrEqualTo(0)
            .When(request => request.MinTotalAmount.HasValue)
            .WithMessage("Minimum total amount must be greater than or equal to zero.");

        RuleFor(request => request.MaxTotalAmount)
            .GreaterThanOrEqualTo(0)
            .When(request => request.MaxTotalAmount.HasValue)
            .WithMessage("Maximum total amount must be greater than or equal to zero.");

        RuleFor(request => request)
            .Must(request =>
                !request.MinTotalAmount.HasValue ||
                !request.MaxTotalAmount.HasValue ||
                request.MinTotalAmount <= request.MaxTotalAmount)
            .WithMessage("Minimum total amount cannot be greater than maximum total amount.");

        RuleFor(request => request)
            .Must(request =>
                !request.StartDate.HasValue ||
                !request.EndDate.HasValue ||
                request.StartDate <= request.EndDate)
            .WithMessage("Start date cannot be greater than end date.");
    }
}