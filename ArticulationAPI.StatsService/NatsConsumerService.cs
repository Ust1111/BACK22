using System.Text.Json;
using NATS.Client.Core;
using NATS.Client.JetStream;
using NATS.Client.JetStream.Models;

namespace ArticulationAPI.StatsService;

public class NatsConsumerService : BackgroundService
{
    private readonly StatsStore _store;
    private readonly IConfiguration _config;
    private readonly ILogger<NatsConsumerService> _logger;

    public NatsConsumerService(StatsStore store, IConfiguration config, ILogger<NatsConsumerService> logger)
    {
        _store = store;
        _config = config;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var url = _config["Nats:Url"] ?? "nats://localhost:4222";

        await using var connection = new NatsConnection(new NatsOpts { Url = url });
        var js = new NatsJSContext(connection);

        var stream = await js.CreateOrUpdateStreamAsync(new StreamConfig("FAVORITES", new[] { "favorites.>" }), stoppingToken);

        var consumer = await stream.CreateOrUpdateConsumerAsync(
            new ConsumerConfig("stats-consumer") { AckPolicy = ConsumerConfigAckPolicy.Explicit },
            stoppingToken);

        _logger.LogInformation("NATS consumer started, listening on favorites.>");

        await foreach (var msg in consumer.ConsumeAsync<byte[]>(cancellationToken: stoppingToken))
        {
            try
            {
                var json = System.Text.Encoding.UTF8.GetString(msg.Data ?? Array.Empty<byte>());
                var evt = JsonSerializer.Deserialize<FavoriteAddedEvent>(json);

                if (evt != null)
                {
                    _store.Increment(evt.UserId, evt.Language);
                    _logger.LogInformation(
                        "Processed favorite event: user {UserId}, language {Language}",
                        evt.UserId, evt.Language);
                }

                await msg.AckAsync(cancellationToken: stoppingToken);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to process message, it will be redelivered");
            }
        }
    }
}