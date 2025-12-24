using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Contracts.Dashboard;
using FinancialManager.Domain.Enums;
using FinancialManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Repositories;

/*
 * Projeto: FinancialManager
 * Camada: Infrastructure
 * Responsabilidade:
 * - Implementar consultas agregadas para o Dashboard (por Pessoa / por Categoria).
 *
 * Observações de design:
 * - Usa consultas com projeção direta para DTOs (select new ...Dto) para evitar carregar entidades completas.
 * - AsNoTracking() em todas as consultas para otimizar leitura (dashboard é consulta, não update).
 * - Utiliza LEFT JOIN (via "join ... into tx") para incluir Pessoas/Categorias mesmo sem transações.
 * - Calcula TotalReceitas/TotalDespesas filtrando por TransactionType.
 * - Usa Sum com cast para decimal? e coalescência (?? 0m) para evitar null quando não há registros.
 */
public sealed class DashboardRepository : IDashboardRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Constrói o repositório de Dashboard.
    /// </summary>
    /// <param name="context">DbContext EF Core.</param>
    public DashboardRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna totais agrupados por Pessoa.
    /// </summary>
    /// <remarks>
    /// Estratégia:
    /// - Parte de Pessoas (fonte principal)
    /// - LEFT JOIN com Transactions para incluir pessoas sem transações
    /// - Calcula somatórios por tipo (Receita/Despesa)
    /// </remarks>
    public async Task<List<TotaisPorPessoaItemDto>> GetTotaisPorPessoaAsync(CancellationToken ct = default)
    {
        var query =
            from p in _context.Pessoas.AsNoTracking()

                // LEFT JOIN: "into tx" cria um agrupamento de transações por pessoa.
            join t in _context.Transactions.AsNoTracking()
                on p.Id equals t.PessoaId into tx

            // Projeção direta para DTO: evita materializar entidades completas.
            select new TotaisPorPessoaItemDto
            {
                PessoaId = p.Id,
                PessoaNome = p.Nome,

                // Somatório com decimal? para retornar null se vazio e depois coalescer para 0.
                TotalReceitas = tx
                    .Where(x => x.Type == TransactionType.Receita)
                    .Sum(x => (decimal?)x.Amount) ?? 0m,

                TotalDespesas = tx
                    .Where(x => x.Type == TransactionType.Despesa)
                    .Sum(x => (decimal?)x.Amount) ?? 0m,

                // Saldo calculado após materializar a lista (mantém leitura simples).
                Saldo = 0m
            };

        var items = await query.ToListAsync(ct);

        // Saldo = Receitas - Despesas
        foreach (var it in items)
            it.Saldo = it.TotalReceitas - it.TotalDespesas;

        return items;
    }

    /// <summary>
    /// Retorna totais agrupados por Categoria.
    /// </summary>
    /// <remarks>
    /// Estratégia:
    /// - Parte de Categories (fonte principal)
    /// - LEFT JOIN com Transactions para incluir categorias sem transações
    /// - Calcula somatórios por tipo (Receita/Despesa)
    /// - Ordena por descrição para facilitar visualização no dashboard
    /// </remarks>
    public async Task<List<TotaisPorCategoriaItemDto>> GetTotaisPorCategoriaAsync(CancellationToken ct = default)
    {
        var query =
            from c in _context.Categories.AsNoTracking()

                // LEFT JOIN: categoria aparece mesmo sem transação vinculada.
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
                    .Sum(x => (decimal?)x.Amount) ?? 0m
            };

        var items = await query.ToListAsync(ct);

        // Calcula saldo após materialização.
        foreach (var i in items)
            i.Saldo = i.TotalReceitas - i.TotalDespesas;

        // Ordenação no retorno facilita consumo no front (cards/tabela/gráfico).
        return items
            .OrderBy(x => x.CategoryDescription)
            .ToList();
    }
}
