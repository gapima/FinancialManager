using FinancialManager.Api.DTOs.Pessoa;
using FinancialManager.Application.Abstractions.Pessoas;
using FinancialManager.Domain.Entities;
using FinancialManager.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

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
    public async Task<ActionResult<List<PessoaResponseDto>>> GetAll(CancellationToken ct)
    {
        var pessoas = await _service.GetAllAsync(ct);

        var dto = pessoas.Select(p => new PessoaResponseDto
        {
            Id = p.Id,
            Nome = p.Nome,
            Idade = p.Idade
        }).ToList();

        return Ok(dto);
    }

    [HttpGet("{id:int}")]
    public async Task<ActionResult<PessoaResponseDto>> GetById(int id, CancellationToken ct)
    {
        var pessoa = await _service.GetByIdAsync(id, ct);
        if (pessoa is null) return NotFound();

        return Ok(new PessoaResponseDto
        {
            Id = pessoa.Id,
            Nome = pessoa.Nome,
            Idade = pessoa.Idade
        });
    }

    [HttpPost]
    public async Task<ActionResult<PessoaResponseDto>> Create(PessoaCreateDto input, CancellationToken ct)
    {
        var pessoa = new Pessoa
        {
            Nome = input.Nome,
            Idade = input.Idade
        };

        var created = await _service.CreateAsync(pessoa, ct);

        return CreatedAtAction(nameof(GetById), new { id = created.Id }, new PessoaResponseDto
        {
            Id = created.Id,
            Nome = created.Nome,
            Idade = created.Idade
        });
    }

    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PessoaUpdateDto input, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, new Pessoa
        {
            Nome = input.Nome,
            Idade = input.Idade
        }, ct);

        return ok ? NoContent() : NotFound();
    }

    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}