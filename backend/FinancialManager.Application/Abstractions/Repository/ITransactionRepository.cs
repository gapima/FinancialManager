using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Abstractions.Repository;

public interface ITransactionRepository
{
    Task<List<Transaction>> GetAllAsync(CancellationToken ct = default);
    Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Transaction> AddAsync(Transaction transaction, CancellationToken ct = default);
    Task<bool> UpdateAsync(Transaction transaction, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    // helpers pra validação de FK (muito importante!)
    Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default);
    Task<bool> PessoaExistsAsync(int pessoaId, CancellationToken ct = default);
}
