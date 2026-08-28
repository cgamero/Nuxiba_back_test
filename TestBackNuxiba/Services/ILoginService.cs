using TestBackNuxiba.DTOs;
using TestBackNuxiba.Models;

namespace TestBackNuxiba.Services;

public interface ILoginService
{
    Task<IEnumerable<Login>> GetAllAsync();
    Task<Login> CreateAsync(CreateLoginDto dto);
    Task<Login?> UpdateAsync(long id, UpdateLoginDto dto);
    Task<bool> DeleteAsync(long id);
}