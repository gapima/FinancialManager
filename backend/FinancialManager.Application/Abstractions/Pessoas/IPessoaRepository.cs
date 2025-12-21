using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Abstractions.Pessoas;

public interface IPessoaRepository
{
    Task<List<Pessoa>> GetAllAsync(CancellationToken ct = default);
    Task<Pessoa?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Pessoa> AddAsync(Pessoa pessoa, CancellationToken ct = default);
    Task<bool> UpdateAsync(Pessoa pessoa, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
