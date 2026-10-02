using ArticulationAPI.Data;
using ArticulationAPI.DTOs;
using ArticulationAPI.Models;
using Microsoft.EntityFrameworkCore;

namespace ArticulationAPI.Services;

public class AuthService
{
    private readonly AppDbContext _db;
    private readonly TokenService _tokens;

    public AuthService(AppDbContext db, TokenService tokens)
    {
        _db = db;
        _tokens = tokens;
    }

    public async Task<AuthResponseDto> RegisterAsync(RegisterDto dto)
    {
        if (await _db.Users.AnyAsync(u => u.Email == dto.Email))
            throw new InvalidOperationException("Email уже используется");

        if (await _db.Users.AnyAsync(u => u.Username == dto.Username))
            throw new InvalidOperationException("Имя пользователя занято");

        var user = new User
        {
            Username     = dto.Username,
            Email        = dto.Email,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(dto.Password),
        };

        _db.Users.Add(user);
        await _db.SaveChangesAsync();

        return new AuthResponseDto(_tokens.GenerateToken(user), user.Username, user.Id);
    }

    public async Task<AuthResponseDto> LoginAsync(LoginDto dto)
    {
        var user = await _db.Users.FirstOrDefaultAsync(u => u.Email == dto.Email)
            ?? throw new InvalidOperationException("Неверный email или пароль");

        if (!BCrypt.Net.BCrypt.Verify(dto.Password, user.PasswordHash))
            throw new InvalidOperationException("Неверный email или пароль");

        return new AuthResponseDto(_tokens.GenerateToken(user), user.Username, user.Id);
    }
}