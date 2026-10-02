using ArticulationAPI.Data;
using ArticulationAPI.DTOs;
using ArticulationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticulationAPI.Services;

public class FavoritesService
{
    private readonly AppDbContext _db;
    private readonly EventPublisher _publisher;

    public FavoritesService(AppDbContext db, EventPublisher publisher)
    {
        _db = db;
        _publisher = publisher;
    }

    public async Task<List<FavoritePairDto>> GetAllAsync(Guid userId)
    {
        return await _db.FavoritePairs
            .Where(f => f.UserId == userId)
            .OrderByDescending(f => f.CreatedAt)
            .Select(f => new FavoritePairDto(
                f.Id,
                f.LeftText, f.LeftLanguage, f.LeftTranscription,
                f.RightText, f.RightLanguage, f.RightTranscription))
            .ToListAsync();
    }
    public IQueryable<FavoritePair> GetAllQueryable(Guid userId)
    {
        return _db.FavoritePairs.Where(f => f.UserId == userId);
    }

    public async Task<FavoritePairDto> AddAsync(Guid userId, FavoritePairDto dto)
    {
        var pair = new FavoritePair
        {
            UserId             = userId,
            LeftText           = dto.LeftText,
            LeftLanguage       = dto.LeftLanguage,
            LeftTranscription  = dto.LeftTranscription,
            RightText          = dto.RightText,
            RightLanguage      = dto.RightLanguage,
            RightTranscription = dto.RightTranscription,
        };

        _db.FavoritePairs.Add(pair);
        await _db.SaveChangesAsync();
        await _publisher.PublishFavoriteAddedAsync(userId, dto.LeftLanguage, dto.LeftText);
        await _publisher.PublishFavoriteAddedAsync(userId, dto.RightLanguage, dto.RightText);

        return dto with { Id = pair.Id };
    }

    public async Task DeleteAsync(Guid userId, Guid pairId)
    {
        var pair = await _db.FavoritePairs
            .FirstOrDefaultAsync(f => f.Id == pairId && f.UserId == userId)
            ?? throw new InvalidOperationException("Не найдено");

        _db.FavoritePairs.Remove(pair);
        await _db.SaveChangesAsync();
    }
}