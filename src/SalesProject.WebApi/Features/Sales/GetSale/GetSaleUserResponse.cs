namespace SalesProject.WebApi.Features.Sales.GetSale;

/// <summary>
/// Represents the user returned in the GetSale operation.
/// </summary>
public class GetSaleUserResponse
{
    /// <summary>
    /// Gets or sets the unique identifier of the user.
    /// </summary>
    public Guid Id { get; set; }

    /// <summary>
    /// Gets or sets the username of the user.
    /// </summary>
    public string Username { get; set; } = string.Empty;

    /// <summary>
    /// Gets or sets the email address of the user.
    /// </summary>
    public string Email { get; set; } = string.Empty;
}
