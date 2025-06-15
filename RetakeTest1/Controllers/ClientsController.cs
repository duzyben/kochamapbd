using Microsoft.AspNetCore.Mvc;
using RetakeTest1.Data.Dtos;
using RetakeTest1.Exceptions;
using RetakeTest1.Services;

namespace RetakeTest1.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ClientsController : ControllerBase
{
    private readonly IDbService _dbService;

    public ClientsController(IDbService dbService)
    {
        this._dbService = dbService;
    }

    [HttpGet("{clientId}")]
    public async Task<IActionResult> GetClient(int clientId)
    {
        try
        {
            var c = await _dbService.GetCustomerById(clientId);
            return Ok(c);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
    }

    [HttpPost]
    public async Task<IActionResult> AddClient(AddClientDto dto)
    {
        int newClientId;
        try
        {
            newClientId = await _dbService.AddNewClient(dto);
        }
        catch (NotFoundException e)
        {
            return NotFound(e.Message);
        }
        catch (ConflictException ex)
        {
            return Conflict(ex.Message);
        }

        return CreatedAtAction(nameof(GetClient), new { id = newClientId }, dto);
    }
}