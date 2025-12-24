using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Domain.Entities;
using FinancialManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Repository;

/*
 * Projeto: FinancialManager
 * Camada: Infrastructure
 * Responsabilidade:
 * - Implementar o contrato ITransactionRepository utilizando Entity Framework Core.
 *
 * Observações de design:
 * - Operações de leitura utilizam AsNoTracking() para melhor performance,
 *   pois não há intenção de alterar os dados retornados.
 * - Operações de Update/Delete trabalham com entidades rastreadas pelo EF.
 * - Métodos auxiliares (CategoryExistsAsync / PessoaExistsAsync) permitem
 *   validação antecipada de FKs na camada Application, evitando exceções genéricas do banco.
 */
public sealed class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Constrói o repositório de Transaction com o DbContext injetado via DI.
    /// </summary>
    /// <param name="db">Contexto EF Core responsável pelo acesso ao banco.</param>
    public TransactionRepository(AppDbContext db)
    {
        _db = db;
    }

    /// <summary>
    /// Retorna todas as transações cadastradas.
    /// </summary>
    /// <remarks>
    /// AsNoTracking(): leitura pura, sem necessidade de change tracking,
    /// reduzindo overhead de memória e CPU.
    /// </remarks>
    public Task<List<Transaction>> GetAllAsync(CancellationToken ct = default)
        => _db.Transactions
            .AsNoTracking()
            .ToListAsync(ct);

    /// <summary>
    /// Retorna uma transação pelo Id.
    /// </summary>
    /// <remarks>
    /// Leitura sem tracking, pois a intenção é apenas consulta.
    /// </remarks>
    public Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Transactions
            .AsNoTracking()
            .FirstOrDefaultAsync(x => x.Id == id, ct);

    /// <summary>
    /// Persiste uma nova transação.
    /// </summary>
    /// <param name="transaction">Entidade de domínio já validada.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Entidade persistida (com Id preenchido).</returns>
    public async Task<Transaction> AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync(ct);
        return transaction;
    }

    /// <summary>
    /// Atualiza uma transação existente.
    /// </summary>
    /// <remarks>
    /// Estratégia utilizada:
    /// - Update direto da entidade
    /// - EF Core marca todos os campos como modificados
    ///
    /// Essa abordagem é adequada quando:
    /// - a entidade já representa o estado final desejado
    /// - não há necessidade de atualização parcial campo a campo
    /// </remarks>
    public async Task<bool> UpdateAsync(Transaction transaction, CancellationToken ct = default)
    {
        _db.Transactions.Update(transaction);

        // SaveChangesAsync retorna o número de linhas afetadas.
        return await _db.SaveChangesAsync(ct) > 0;
    }

    /// <summary>
    /// Remove uma transação pelo Id.
    /// </summary>
    /// <remarks>
    /// A exclusão pode falhar quando:
    /// - a transação não existe
    /// - ocorre algum erro de persistência
    /// </remarks>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        // Aqui buscamos entidade rastreada para permitir remoção via EF.
        var entity = await _db.Transactions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        _db.Transactions.Remove(entity);
        return await _db.SaveChangesAsync(ct) > 0;
    }

    /// <summary>
    /// Verifica se uma categoria existe.
    /// </summary>
    /// <remarks>
    /// Método auxiliar usado pela camada Application para validação antecipada de FK.
    /// </remarks>
    public Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default)
        => _db.Categories.AnyAsync(c => c.Id == categoryId, ct);

    /// <summary>
    /// Verifica se uma pessoa existe.
    /// </summary>
    /// <remarks>
    /// Método auxiliar usado pela camada Application para validação antecipada de FK.
    /// </remarks>
    public Task<bool> PessoaExistsAsync(int pessoaId, CancellationToken ct = default)
        => _db.Pessoas.AnyAsync(p => p.Id == pessoaId, ct);
}
