using FinancialManager.Domain.Enums;

namespace FinancialManager.Domain.Entities;

public class Transaction
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public decimal Amount { get; set; }
    public TransactionType Type { get; set; }

    public int CategoryId { get; set; }
    public int PessoaId { get; set; }

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
}
