using FinancialManager.Application.Contracts.Transaction;

namespace FinancialManager.Application.Abstractions.Service;

public interface ITransactionService
{
    Task<List<TransactionResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task<TransactionResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<TransactionResponseDto> CreateAsync(TransactionCreateDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, TransactionUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
