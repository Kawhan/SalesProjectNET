using FluentValidation;

namespace SalesProject.WebApi.Features.Products.GetAllProducts;


/// <summary>
/// Validator for GetAllProductsRequest.
/// </summary>
public class GetAllProductsRequestValidator : AbstractValidator<GetAllProductsRequest>
{
    /// <summary>
    /// Initializes a new instance of the GetAllProductsRequestValidator with defined validation rules.
    /// </summary>
    public GetAllProductsRequestValidator()
    {
        RuleFor(request => request.PageNumber)
            .GreaterThan(0)
            .WithMessage("Page number must be greater than zero.");

        RuleFor(request => request.PageSize)
            .GreaterThan(0)
            .WithMessage("Page size must be greater than zero.")
            .LessThanOrEqualTo(100)
            .WithMessage("Page size must be less than or equal to 100.");

        RuleFor(request => request.MinPrice)
            .GreaterThanOrEqualTo(0)
            .When(request => request.MinPrice.HasValue)
            .WithMessage("Minimum price must be greater than or equal to zero.");

        RuleFor(request => request.MaxPrice)
            .GreaterThanOrEqualTo(0)
            .When(request => request.MaxPrice.HasValue)
            .WithMessage("Maximum price must be greater than or equal to zero.");

        RuleFor(request => request)
            .Must(request =>
                !request.MinPrice.HasValue ||
                !request.MaxPrice.HasValue ||
                request.MinPrice <= request.MaxPrice)
            .WithMessage("Minimum price cannot be greater than maximum price.");
    }
}