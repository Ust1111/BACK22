namespace ArticulationAPI.Models;
using System.Text.Json.Serialization;

public class FavoritePair
{
    public Guid Id { get; set; } = Guid.NewGuid();
    public Guid UserId { get; set; }

    // Левое окно
    public string LeftText { get; set; } = string.Empty;
    public string LeftLanguage { get; set; } = string.Empty;
    public string? LeftTranscription { get; set; }  // null = авто, строка = ручная правка

    // Правое окно
    public string RightText { get; set; } = string.Empty;
    public string RightLanguage { get; set; } = string.Empty;
    public string? RightTranscription { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    [JsonIgnore]
    public User User { get; set; } = null!;
}