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
        => _service = service;

    [HttpGet]
    public async Task<ActionResult<List<PessoaResponseDto>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PessoaResponseDto>> GetById(int id, CancellationToken ct)
    {
        var pessoa = await _service.GetByIdAsync(id, ct);
        return pessoa is null ? NotFound() : Ok(pessoa);
    }

    [HttpPost]
    public async Task<ActionResult<PessoaResponseDto>> Create([FromBody] PessoaCreateDto input, CancellationToken ct)
    {
        var created = await _service.CreateAsync(input, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, [FromBody] PessoaUpdateDto input, CancellationToken ct)
        => await _service.UpdateAsync(id, input, ct) ? NoContent() : NotFound();

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
