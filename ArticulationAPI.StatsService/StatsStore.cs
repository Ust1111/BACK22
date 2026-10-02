using System.Collections.Concurrent;

namespace ArticulationAPI.StatsService;

public class StatsStore
{
    private readonly ConcurrentDictionary<(Guid UserId, string Language), int> _counts = new();

    public void Increment(Guid userId, string language)
    {
        _counts.AddOrUpdate((userId, language), 1, (_, current) => current + 1);
    }

    public Dictionary<string, int> GetForUser(Guid userId)
    {
        return _counts
            .Where(kv => kv.Key.UserId == userId)
            .ToDictionary(kv => kv.Key.Language, kv => kv.Value);
    }
}