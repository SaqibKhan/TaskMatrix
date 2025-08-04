using Microsoft.AspNetCore.Mvc;
using TaskMatrix.Application.DTOs;
using TaskMatrix.Application.Interfaces;

namespace TaskMatrix.WebAPI.Controllers;

[ApiController]
[Route("[controller]")]
public class AppTaskController : ControllerBase
{
    private readonly IAppTaskService _iAppTaskService;
    private readonly ILogger<AppTaskController> _logger;

    public AppTaskController(ILogger<AppTaskController> logger, IAppTaskService iAppTaskService)
    {
        _logger = logger;
        _iAppTaskService = iAppTaskService;
    }

    [HttpGet]
    public async Task<ActionResult<IEnumerable<AppTaskDto>>> GetAll()
    {
        var products = await _iAppTaskService.GetAllAsync();
        return Ok(products);
    }

    [HttpGet("{id}")]
    public async Task<ActionResult<AppTaskDto>> GetById(int id)
    {
        var product = await _iAppTaskService.GetByIdAsync(id);
        return Ok(product);
    }

    [HttpPost]
    public async Task<ActionResult<AppTaskDto>> Create(CreateAppTaskDto dto)
    {
        var product = await _iAppTaskService.CreateAsync(dto);
        return CreatedAtAction(nameof(GetById), new { id = product.Id }, product);
    }

    [HttpPost("bulk-create")]
    public async Task<IActionResult> BatchCreate([FromBody] List<CreateAppTaskDto> dtos)
    {
        if (dtos == null || !dtos.Any())
            return BadRequest("No items provided for update.");

        foreach (var dto in dtos)
        {
            var product = await _iAppTaskService.CreateAsync(dto);
        }

        return NoContent();
    }

    [HttpPut()]
    public async Task<IActionResult> Update(UpdateAppTaskDto dto)
    {
        await _iAppTaskService.UpdateAsync(dto.Id, dto);
        return NoContent();
    }

    [HttpDelete("{id}")]
    public async Task<IActionResult> Delete(int id)
    {
        await _iAppTaskService.DeleteAsync(id);
        return NoContent();
    }

    [HttpGet("paged")]
    public async Task<ActionResult<IEnumerable<AppTaskDto>>> GetPaged([FromQuery] int skip = 0, [FromQuery] int take = 20)
    {
        var tasks = await _iAppTaskService.GetPagedAsync(skip, take);
        return Ok(tasks);
    }
}
