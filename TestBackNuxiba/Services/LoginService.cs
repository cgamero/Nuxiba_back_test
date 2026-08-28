using Microsoft.EntityFrameworkCore;
using TestBackNuxiba.Data;
using TestBackNuxiba.DTOs;
using TestBackNuxiba.Models;

namespace TestBackNuxiba.Services;

public class LoginService : ILoginService
{
    private readonly CCenterDbContext _context;

    public LoginService(CCenterDbContext context)
    {
        _context = context;
    }

    private async Task ValidateMovementAsync(
    int userId,
    int tipoMov,
    DateTime fecha,
    long? excludedLoginId = null)
    {
        // Validate user
        var userExists = await _context.Users
            .AnyAsync(u => u.User_id == userId);

        if (!userExists)
        {
            throw new KeyNotFoundException(
                $"User with id {userId} does not exist.");
        }

        var movementsQuery = _context.Logins
            .Where(l => l.User_id == userId);

        if (excludedLoginId.HasValue)
        {
            movementsQuery = movementsQuery
                .Where(l => l.LogLoginId != excludedLoginId.Value);
        }

        var movements = await movementsQuery
            .OrderBy(l => l.fecha)
            .ThenBy(l => l.LogLoginId)
            .ToListAsync();

        // Add the movement we're validating
        movements.Add(new Login
        {
            LogLoginId = excludedLoginId ?? long.MaxValue,
            User_id = userId,
            TipoMov = tipoMov,
            fecha = fecha
        });

        movements = movements
            .OrderBy(l => l.fecha)
            .ThenBy(l => l.LogLoginId)
            .ToList();

        Login? previousMovement = null;

        foreach (var movement in movements)
        {
            if (previousMovement != null)
            {
                if (previousMovement.TipoMov == movement.TipoMov)
                {
                    if (movement.TipoMov == 1)
                    {
                        throw new InvalidOperationException(
                            "The user cannot register a login without a previous logout.");
                    }

                    throw new InvalidOperationException(
                        "The user cannot register a logout without a previous login.");
                }
            }

            previousMovement = movement;
        }

        // The first movement must always be a login.
        if (movements.Count > 0 && movements[0].TipoMov != 1)
        {
            throw new InvalidOperationException(
                "The first movement for a user must be a login.");
        }
    }

    public async Task<IEnumerable<Login>> GetAllAsync()
    {
        return await _context.Logins
            .AsNoTracking()
            .OrderByDescending(l => l.fecha)
            .ToListAsync();
    }

    public async Task<Login> CreateAsync(CreateLoginDto dto)
    {
        await ValidateMovementAsync(
            dto.User_id,
            dto.TipoMov,
            dto.fecha);

        var login = new Login
        {
            User_id = dto.User_id,
            Extension = dto.Extension,
            TipoMov = dto.TipoMov,
            fecha = dto.fecha
        };

        _context.Logins.Add(login);

        await _context.SaveChangesAsync();

        return login;
    }

    public async Task<Login?> UpdateAsync(
    long id,
    UpdateLoginDto dto)
    {
        var login = await _context.Logins
            .FirstOrDefaultAsync(l => l.LogLoginId == id);

        if (login == null)
        {
            return null;
        }

        await ValidateMovementAsync(
            dto.User_id,
            dto.TipoMov,
            dto.fecha,
            id);

        login.User_id = dto.User_id;
        login.Extension = dto.Extension;
        login.TipoMov = dto.TipoMov;
        login.fecha = dto.fecha;

        await _context.SaveChangesAsync();

        return login;
    }

    public async Task<Login> CreateAsync2(CreateLoginDto dto)
    {
        // 1. Validate that the user exists
        var userExists = await _context.Users
            .AnyAsync(u => u.User_id == dto.User_id);

        if (!userExists)
        {
            throw new KeyNotFoundException(
                $"User with id {dto.User_id} does not exist.");
        }

        // 2. Validate the date
        /*if (dto.fecha > DateTime.Now)
        {
            throw new ArgumentException(
                "The login/logout date cannot be in the future.");
        }*/

        // 3. Check the user's current session
        var lastMovement = await _context.Logins
            .Where(l => l.User_id == dto.User_id)
            .OrderByDescending(l => l.fecha)
            .FirstOrDefaultAsync();

        // 4. Validate LOGIN
        if (dto.TipoMov == 1)
        {
            if (lastMovement != null && lastMovement.TipoMov == 1)
            {
                throw new InvalidOperationException(
                    "The user is already logged in. A logout is required before registering a new login.");
            }
        }

        // 5. Validate LOGOUT
        if (dto.TipoMov == 0)
        {
            if (lastMovement == null || lastMovement.TipoMov == 0)
            {
                throw new InvalidOperationException(
                    "The user does not have an active login session.");
            }
        }

        // 6. Create entity
        var login = new Login
        {
            User_id = dto.User_id,
            Extension = dto.Extension,
            TipoMov = dto.TipoMov,
            fecha = dto.fecha
        };

        // 7. Save
        _context.Logins.Add(login);

        await _context.SaveChangesAsync();

        return login;
    }

    public async Task<Login?> UpdateAsync2(long id, UpdateLoginDto dto)
    {
        // 1. Find the existing record
        var login = await _context.Logins
            .FirstOrDefaultAsync(l => l.LogLoginId == id);

        if (login == null)
        {
            return null;
        }

        // 2. Validate that the user exists
        var userExists = await _context.Users
            .AnyAsync(u => u.User_id == dto.User_id);

        if (!userExists)
        {
            throw new KeyNotFoundException(
                $"User with id {dto.User_id} does not exist.");
        }

        // 3. Update the entity
        login.User_id = dto.User_id;
        login.Extension = dto.Extension;
        login.TipoMov = dto.TipoMov;
        login.fecha = dto.fecha;

        await _context.SaveChangesAsync();

        return login;
    }

    public async Task<bool> DeleteAsync(long id)
    {
        var login = await _context.Logins
            .FirstOrDefaultAsync(l => l.LogLoginId == id);

        if (login == null)
        {
            return false;
        }

        _context.Logins.Remove(login);

        await _context.SaveChangesAsync();

        return true;
    }
}