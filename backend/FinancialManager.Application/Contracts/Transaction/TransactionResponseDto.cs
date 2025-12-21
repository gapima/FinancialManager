using FinancialManager.Domain.Enums;

namespace FinancialManager.Application.Contracts.Transaction;

public sealed class TransactionResponseDto
{
    public int Id { get; init; }
    public string Description { get; init; } = string.Empty;
    public string Amount { get; init; } = string.Empty;
    public TransactionType Type { get; init; }
    public int CategoryId { get; init; }
    public int PessoaId { get; init; }
    public DateTime CreatedAt { get; init; }
}
