namespace NoWasteOfMoney.Models.Dtos
{
    public record UserResponse(
        Guid Id,
        Guid PersonId,
        string Name,
        string Email,
        string Role,
        DateTime CreatedAt
    );
}
