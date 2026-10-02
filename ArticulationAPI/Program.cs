using System.Text;
using ArticulationAPI.Data;
using ArticulationAPI.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using ArticulationAPI.TranscriptionService.Grpc;
using Microsoft.AspNetCore.OData;

var builder = WebApplication.CreateBuilder(args);
builder.Services.AddSingleton<EventPublisher>();

builder.Services.AddControllers().AddOData(options =>
    options.Select().Filter().OrderBy().SetMaxTop(100).Count());
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen(c =>
{
    c.AddSecurityDefinition("Bearer", new Microsoft.OpenApi.OpenApiSecurityScheme
    {
        In = Microsoft.OpenApi.ParameterLocation.Header,
        Description = "Введите: Bearer {токен}",
        Name = "Authorization",
        Type = Microsoft.OpenApi.SecuritySchemeType.Http,
        BearerFormat = "JWT",
        Scheme = "Bearer"
    });
    c.AddSecurityRequirement(document => new Microsoft.OpenApi.OpenApiSecurityRequirement
    {
        [new Microsoft.OpenApi.OpenApiSecuritySchemeReference("Bearer", document)] = new List<string>()
    });
});

// PostgreSQL
builder.Services.AddDbContext<AppDbContext>(o =>
    o.UseNpgsql(builder.Configuration.GetConnectionString("Default")));

// Сервисы
builder.Services.AddScoped<TokenService>();
builder.Services.AddScoped<AuthService>();
builder.Services.AddScoped<FavoritesService>();
builder.Services.AddHttpClient();
builder.Services.AddSingleton<IHttpContextAccessor, HttpContextAccessor>();

//gRPC-клиент к сервису транскрипции
builder.Services.AddGrpcClient<TranscriptionGrpc.TranscriptionGrpcClient>(o =>
{
    o.Address = new Uri(builder.Configuration["Transcription:GrpcUrl"]
        ?? "http://localhost:5099");
});

// JWT
var jwtSecret = builder.Configuration["Jwt:Secret"]!;
builder.Services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
    .AddJwtBearer(o =>
    {
        o.TokenValidationParameters = new TokenValidationParameters
        {
            ValidateIssuer           = true,
            ValidateAudience         = true,
            ValidateLifetime         = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer              = builder.Configuration["Jwt:Issuer"],
            ValidAudience            = builder.Configuration["Jwt:Audience"],
            IssuerSigningKey         = new SymmetricSecurityKey(
                Encoding.UTF8.GetBytes(jwtSecret)),
        };
    });

builder.Services.AddAuthorization();

// CORS
builder.Services.AddCors(o => o.AddPolicy("ReactApp", p =>
    p.WithOrigins("http://localhost:3000", "https://localhost:3000")
     .AllowAnyHeader()
     .AllowAnyMethod()));

var app = builder.Build();

using (var scope = app.Services.CreateScope())
{
    var publisher = scope.ServiceProvider.GetRequiredService<EventPublisher>();
    await publisher.EnsureStreamAsync();
}

// Автоматически применяем миграции при старте
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<AppDbContext>();
    db.Database.Migrate();
}

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.UseCors("ReactApp");
app.UseAuthentication();
app.UseAuthorization();
app.MapControllers();
app.Run();