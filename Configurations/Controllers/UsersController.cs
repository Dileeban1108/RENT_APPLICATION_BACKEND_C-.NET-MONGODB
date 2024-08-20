using Microsoft.AspNetCore.Mvc;
using RentApplication.Models;
using RentApplication.Services;

namespace RentApplication.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly RentApplicationService _rentApplicationService;

    public UsersController(RentApplicationService rentApplicationService) =>
        _rentApplicationService = rentApplicationService;

    [HttpGet]
    public async Task<List<User>> Get() =>
        await _rentApplicationService.GetUsersAsync();

    [HttpGet("{id:length(24)}")]
    public async Task<ActionResult<User>> Get(string id)
    {
        var user = await _rentApplicationService.GetUserAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        return user;
    }

    [HttpPost]
    public async Task<IActionResult> Post(User newUser)
    {
        await _rentApplicationService.CreateUserAsync(newUser);

        return CreatedAtAction(nameof(Get), new { id = newUser.Id }, newUser);
    }

    [HttpPut("{id:length(24)}")]
    public async Task<IActionResult> Update(string id, User updatedUser)
    {
        var user = await _rentApplicationService.GetUserAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        updatedUser.Id = user.Id;

        await _rentApplicationService.UpdateUserAsync(id, updatedUser);

        return NoContent();
    }

    [HttpDelete("{id:length(24)}")]
    public async Task<IActionResult> Delete(string id)
    {
        var user = await _rentApplicationService.GetUserAsync(id);

        if (user is null)
        {
            return NotFound();
        }

        await _rentApplicationService.DeleteUserAsync(id);

        return NoContent();
    }
}
