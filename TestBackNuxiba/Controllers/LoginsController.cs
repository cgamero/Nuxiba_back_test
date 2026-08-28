using Microsoft.AspNetCore.Mvc;
using TestBackNuxiba.DTOs;
using TestBackNuxiba.Services;

namespace TestBackNuxiba.Controllers;

[ApiController]
[Route("logins")]
public class LoginsController : ControllerBase
{
    private readonly ILoginService _loginService;

    public LoginsController(ILoginService loginService)
    {
        _loginService = loginService;
    }

    [HttpGet]
    public async Task<IActionResult> GetAll()
    {
        var logins = await _loginService.GetAllAsync();

        return Ok(logins);
    }

    [HttpPost]
    public async Task<IActionResult> Create(CreateLoginDto dto)
    {
        try
        {
            var login = await _loginService.CreateAsync(dto);

            return CreatedAtAction(
                nameof(GetAll),
                null,
                login);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
        catch (ArgumentException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
        catch (InvalidOperationException ex)
        {
            return BadRequest(new
            {
                message = ex.Message
            });
        }
    }

    [HttpPut("{id:long}")]
    public async Task<IActionResult> Update(
    long id,
    UpdateLoginDto dto)
    {
        try
        {
            var login = await _loginService.UpdateAsync(id, dto);

            if (login == null)
            {
                return NotFound(new
                {
                    message = $"Login record with id {id} was not found."
                });
            }

            return Ok(login);
        }
        catch (KeyNotFoundException ex)
        {
            return NotFound(new
            {
                message = ex.Message
            });
        }
    }

    [HttpDelete("{id:long}")]
    public async Task<IActionResult> Delete(long id)
    {
        var deleted = await _loginService.DeleteAsync(id);

        if (!deleted)
        {
            return NotFound(new
            {
                message = $"Login record with id {id} was not found."
            });
        }

        return NoContent();
    }
}