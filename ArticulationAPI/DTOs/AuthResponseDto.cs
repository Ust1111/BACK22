namespace ArticulationAPI.DTOs;

public record AuthResponseDto(string Token, string Username, Guid UserId);