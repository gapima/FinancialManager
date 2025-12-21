using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class TransactionController : ControllerBase
{
    private readonly ITransactionService _service;

    public TransactionController(ITransactionService service)
    {
        _service = service;
    }

    [HttpGet]
    public async Task<ActionResult<List<TransactionResponseDto>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    [HttpGet("{id:int}")]
    public async Task<ActionResult<TransactionResponseDto>> GetById(int id, CancellationToken ct)
    {
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    [HttpPost]
    public async Task<ActionResult<TransactionResponseDto>> Create(TransactionCreateDto input, CancellationToken ct)
    {
        var created = await _service.CreateAsync(input, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, TransactionUpdateDto input, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, input, ct);
        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
