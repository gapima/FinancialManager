using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Domain.Entities;
using FinancialManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Repositories;

/*
 * Projeto: FinancialManager
 * Camada: Infrastructure
 * Responsabilidade:
 * - Implementar o contrato IPessoaRepository usando Entity Framework Core.
 *
 * Observações de design:
 * - Queries de leitura usam AsNoTracking() para melhorar performance
 *   e evitar overhead de change tracking quando não há intenção de alteração.
 * - Operações de Update/Delete buscam a entidade rastreada (tracking),
 *   pois o EF precisa acompanhar mudanças para persistir corretamente.
 * - A persistência é mantida isolada nesta camada para preservar o desacoplamento
 *   da Application em relação ao EF Core / DB.
 */
public sealed class PessoaRepository : IPessoaRepository
{
    private readonly AppDbContext _context;

    /// <summary>
    /// Constrói o repositório de Pessoa com o DbContext injetado via DI. 
    /// </summary>
    /// <param name="context">Contexto EF Core responsável pelo acesso ao banco.</param>
    public PessoaRepository(AppDbContext context)
    {
        _context = context;
    }

    /// <summary>
    /// Retorna todas as pessoas cadastradas.
    /// </summary>
    /// <remarks>
    /// AsNoTracking(): leitura pura (sem necessidade de rastrear mudanças),
    /// reduz uso de memória e melhora desempenho.
    /// </remarks>
    public Task<List<Pessoa>> GetAllAsync(CancellationToken ct = default)
        => _context.Pessoas
            .AsNoTracking()
            .ToListAsync(ct);

    /// <summary>
    /// Retorna uma pessoa pelo Id.
    /// </summary>
    /// <remarks>
    /// AsNoTracking() aqui também é apropriado, pois o objetivo é somente leitura.
    /// </remarks>
    public Task<Pessoa?> GetByIdAsync(int id, CancellationToken ct = default)
        => _context.Pessoas
            .AsNoTracking()
            .FirstOrDefaultAsync(p => p.Id == id, ct);

    /// <summary>
    /// Adiciona uma nova pessoa e persiste no banco.
    /// </summary>
    /// <param name="pessoa">Entidade de domínio a ser persistida.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>A própria entidade persistida (com Id preenchido após SaveChanges).</returns>
    public async Task<Pessoa> AddAsync(Pessoa pessoa, CancellationToken ct = default)
    {
        // Add() registra a entidade para inserção; o Id geralmente é gerado no SaveChanges.
        _context.Pessoas.Add(pessoa);
        await _context.SaveChangesAsync(ct);
        return pessoa;
    }

    /// <summary>
    /// Atualiza dados básicos da pessoa (Nome e Idade).
    /// </summary>
    /// <remarks>
    /// Estratégia utilizada:
    /// - Buscar a entidade rastreada (tracking)
    /// - Aplicar alterações campo a campo
    /// - Persistir com SaveChangesAsync
    ///
    /// Isso evita updates "cegos" e facilita retorno booleano quando o registro não existe.
    /// </remarks>
    public async Task<bool> UpdateAsync(Pessoa pessoa, CancellationToken ct = default)
    {
        // Sem AsNoTracking(): precisamos de uma entidade rastreada para o EF detectar mudanças.
        var existing = await _context.Pessoas.FirstOrDefaultAsync(p => p.Id == pessoa.Id, ct);
        if (existing is null) return false;

        // Atualização explícita: deixa claro quais campos são alterados.
        existing.Nome = pessoa.Nome;
        existing.Idade = pessoa.Idade;

        await _context.SaveChangesAsync(ct);
        return true;
    }

    /// <summary>
    /// Remove uma pessoa pelo Id.
    /// </summary>
    /// <remarks>
    /// Busca a entidade para garantir existência e permitir remoção rastreada pelo EF.
    /// </remarks>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var existing = await _context.Pessoas.FirstOrDefaultAsync(p => p.Id == id, ct);
        if (existing is null) return false;

        _context.Pessoas.Remove(existing);
        await _context.SaveChangesAsync(ct);
        return true;
    }
}
