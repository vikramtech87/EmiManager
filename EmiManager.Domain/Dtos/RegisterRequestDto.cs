namespace EmiManager.Domain.Dtos;

public record RegisterRequestDto(
    string Email,
    string Password,
    string Name);
