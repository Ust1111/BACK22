using ArticulationAPI.StatsService;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddSingleton<StatsStore>();
builder.Services.AddHostedService<NatsConsumerService>();

var app = builder.Build();

app.MapGet("/stats/{userId:guid}", (Guid userId, StatsStore store) =>
{
    var stats = store.GetForUser(userId);
    return Results.Ok(stats);
});

app.MapGet("/", () => "StatsService is running.");

app.Run();