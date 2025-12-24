using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Contracts.Dashboard;
using FinancialManager.Domain.Enums;
using FinancialManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Repositories;

public sealed class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    public async Task<List<TotaisPorPessoaItemDto>> GetTotaisPorPessoaAsync(CancellationToken ct = default)
    {
        // Importante: isso pressupõe que Transaction.Amount é DECIMAL no banco/entidade.
        // Se ainda for string, veja o bloco "OBS Amount string" no final.
        var query =
            from p in _context.Pessoas.AsNoTracking()
            join t in _context.Transactions.AsNoTracking() on p.Id equals t.PessoaId into tx
            select new TotaisPorPessoaItemDto
            {
                PessoaId = p.Id,
                PessoaNome = p.Nome,
                TotalReceitas = tx.Where(x => x.Type == TransactionType.Receita).Sum(x => (decimal?)x.Amount) ?? 0m,
                TotalDespesas = tx.Where(x => x.Type == TransactionType.Despesa).Sum(x => (decimal?)x.Amount) ?? 0m,
                Saldo = 0m // calcula depois
            };

        var items = await query.ToListAsync(ct);

        foreach (var it in items)
            it.Saldo = it.TotalReceitas - it.TotalDespesas;

        return items;
    }

    public async Task<List<TotaisPorCategoriaItemDto>> GetTotaisPorCategoriaAsync(CancellationToken ct = default)
    {
        // Ajuste nomes: _db.Categories / _db.Transactions conforme seu DbSet
        var query =
            from c in _context.Categories.AsNoTracking()
            join t in _context.Transactions.AsNoTracking()
                on c.Id equals t.CategoryId into tx
            select new TotaisPorCategoriaItemDto
            {
                CategoryId = c.Id,
                CategoryDescription = c.Description,

                TotalReceitas = tx
                    .Where(x => x.Type == TransactionType.Receita)
                    .Sum(x => (decimal?)x.Amount) ?? 0m,

                TotalDespesas = tx
                    .Where(x => x.Type == TransactionType.Despesa)
                    .Sum(x => (decimal?)x.Amount) ?? 0m,
            };

        var items = await query.ToListAsync(ct);

        // calcula saldo por item
        foreach (var i in items)
            i.Saldo = i.TotalReceitas - i.TotalDespesas;

        return items
            .OrderBy(x => x.CategoryDescription)
            .ToList();
    }
}
