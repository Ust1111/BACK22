namespace ArticulationAPI.DTOs;

public record FavoritePairDto(
    Guid? Id,
    string LeftText,
    string LeftLanguage,
    string? LeftTranscription,
    string RightText,
    string RightLanguage,
    string? RightTranscription
);