using FinancialManager.Api.DTOs.Pessoa;
using FinancialManager.Domain.Entities;
using FinancialManager.Infrastructure.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Api.Controllers;

[ApiController]
[Route("api/pessoa")]
public class PessoaController : ControllerBase
{
    private readonly AppDbContext _context;

    public PessoaController(AppDbContext context)
    {
        _context = context;
    }

    // GET /api/pessoa
    [HttpGet]
    public async Task<ActionResult<IEnumerable<PessoaResponseDto>>> GetAll()
    {
        var pessoas = await _context.Pessoas
            .Select(p => new PessoaResponseDto
            {
                Id = p.Id,
                Nome = p.Nome,
                Idade = p.Idade
            })
            .ToListAsync();

        return Ok(pessoas);
    }

    // GET /api/pessoa/{id}
    [HttpGet("{id:int}")]
    public async Task<ActionResult<PessoaResponseDto>> GetById(int id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa is null)
            return NotFound();

        return Ok(new PessoaResponseDto
        {
            Id = pessoa.Id,
            Nome = pessoa.Nome,
            Idade = pessoa.Idade
        });
    }

    // POST /api/pessoa
    [HttpPost]
    public async Task<ActionResult<PessoaResponseDto>> Create(PessoaCreateDto dto)
    {
        var pessoa = new Pessoa
        {
            Nome = dto.Nome,
            Idade = dto.Idade
        };

        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync();

        return CreatedAtAction(
            nameof(GetById),
            new { id = pessoa.Id },
            new PessoaResponseDto
            {
                Id = pessoa.Id,
                Nome = pessoa.Nome,
                Idade = pessoa.Idade
            }
        );
    }

    // PUT /api/pessoa/{id}
    [HttpPut("{id:int}")]
    public async Task<IActionResult> Update(int id, PessoaUpdateDto dto)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa is null)
            return NotFound();

        pessoa.Nome = dto.Nome;
        pessoa.Idade = dto.Idade;

        await _context.SaveChangesAsync();
        return NoContent();
    }

    // DELETE /api/pessoa/{id}
    [HttpDelete("{id:int}")]
    public async Task<IActionResult> Delete(int id)
    {
        var pessoa = await _context.Pessoas.FindAsync(id);

        if (pessoa is null)
            return NotFound();

        _context.Pessoas.Remove(pessoa);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
