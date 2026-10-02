namespace ArticulationAPI.StatsService;

public class FavoriteAddedEvent
{
    public Guid EventId { get; set; }
    public DateTime OccurredAt { get; set; }
    public Guid UserId { get; set; }
    public string Language { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
}