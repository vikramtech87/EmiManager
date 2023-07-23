namespace EmiManager.Domain.Dtos;

public record LoginResponseDto(
    string Token,
    string Email,
    string Name);