using FinancialManager.Application.Abstractions.Pessoas;
using FinancialManager.Domain.Entities;
using FinancialManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Repositories;

public sealed class PessoaRepository : IPessoaRepository
{
    private readonly AppDbContext _context;

    public PessoaRepository(AppDbContext context)
    {
        _context = context;
    }

    public Task<List<Pessoa>> GetAllAsync(CancellationToken ct = default)
        => _context.Pessoas.AsNoTracking().ToListAsync(ct);

    public Task<Pessoa?> GetByIdAsync(int id, CancellationToken ct = default)
        => _context.Pessoas.AsNoTracking().FirstOrDefaultAsync(p => p.Id == id, ct);

    public async Task<Pessoa> AddAsync(Pessoa pessoa, CancellationToken ct = default)
    {
        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync(ct);
        return pessoa;
    }

    public async Task<bool> UpdateAsync(Pessoa pessoa, CancellationToken ct = default)
    {
        var existing = await _context.Pessoas.FirstOrDefaultAsync(p => p.Id == pessoa.Id, ct);
        if (existing is null) return false;

        existing.Nome = pessoa.Nome;
        existing.Idade = pessoa.Idade;

        await _context.SaveChangesAsync(ct);
        return true;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _context.Pessoas.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (existing is null) return false;

        _context.Pessoas.Remove(existing);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}