using System.Text;
using System.Text.Json;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;

namespace ArticulationAPI.Services;

public class EventPublisher : IAsyncDisposable
{
    private readonly NatsConnection _connection;
    private readonly NatsJSContext _js;

    public EventPublisher(IConfiguration config)
    {
        var url = config["Nats:Url"] ?? "nats://localhost:4222";
        _connection = new NatsConnection(new NatsOpts { Url = url });
        _js = new NatsJSContext(_connection);
    }

    public async Task EnsureStreamAsync()
    {
        await _js.CreateOrUpdateStreamAsync(new StreamConfig("FAVORITES", new[] { "favorites.>" }));
    }

    public async Task PublishFavoriteAddedAsync(Guid userId, string language, string text)
    {
        var evt = new
        {
            EventId = Guid.NewGuid(),
            OccurredAt = DateTime.UtcNow,
            UserId = userId,
            Language = language,
            Text = text
        };

        var json = JsonSerializer.Serialize(evt);
        var bytes = Encoding.UTF8.GetBytes(json);

        await _js.PublishAsync($"favorites.{language}", bytes);
    }

    public async ValueTask DisposeAsync()
    {
        await _connection.DisposeAsync();
    }
}