using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Pessoa;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class PessoaController : ControllerBase
{
    private readonly IPessoaService _service;

    public PessoaController(IPessoaService service)
    {
        _service = service;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<PessoaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PessoaResponseDto>>> GetAll(CancellationToken ct)
    {
        var pessoas = await _service.GetAllAsync(ct);
        return Ok(pessoas);
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PessoaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PessoaResponseDto>> GetById(int id, CancellationToken ct)
    {
        var pessoa = await _service.GetByIdAsync(id, ct);
        return pessoa is null ? NotFound() : Ok(pessoa);
    }

    [HttpPost]
    [ProducesResponseType(typeof(PessoaResponseDto), StatusCodes.Status201Created)]
    public async Task<ActionResult<PessoaResponseDto>> Create([FromBody] PessoaCreateDto input, CancellationToken ct)
    {
        var created = await _service.CreateAsync(input, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Update(int id, [FromBody] PessoaUpdateDto input, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, input, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
