using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Domain.Entities;
using FinancialManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Repository;

public sealed class TransactionRepository : ITransactionRepository
{
    private readonly AppDbContext _db;

    public TransactionRepository(AppDbContext db)
    {
        _db = db;
    }

    public Task<List<Transaction>> GetAllAsync(CancellationToken ct = default)
        => _db.Transactions.AsNoTracking().ToListAsync(ct);

    public Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Transactions.AsNoTracking().FirstOrDefaultAsync(x => x.Id == id, ct);

    public async Task<Transaction> AddAsync(Transaction transaction, CancellationToken ct = default)
    {
        _db.Transactions.Add(transaction);
        await _db.SaveChangesAsync(ct);
        return transaction;
    }

    public async Task<bool> UpdateAsync(Transaction transaction, CancellationToken ct = default)
    {
        _db.Transactions.Update(transaction);
        return await _db.SaveChangesAsync(ct) > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Transactions.FirstOrDefaultAsync(x => x.Id == id, ct);
        if (entity is null) return false;

        _db.Transactions.Remove(entity);
        return await _db.SaveChangesAsync(ct) > 0;
    }

    public Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default)
        => _db.Categories.AnyAsync(c => c.Id == categoryId, ct);

    public Task<bool> PessoaExistsAsync(int pessoaId, CancellationToken ct = default)
        => _db.Pessoas.AnyAsync(p => p.Id == pessoaId, ct);
}
