using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Domain.Entities;
using FinancialManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Repositories;

public sealed class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    public CategoryRepository(AppDbContext db) => _db = db;

    public Task<List<Category>> GetAllAsync(CancellationToken ct = default)
        => _db.Categories.AsNoTracking().ToListAsync(ct);

    public Task<Category?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Categories.AsNoTracking().FirstOrDefaultAsync(c => c.Id == id, ct);

    public async Task<Category> AddAsync(Category category, CancellationToken ct = default)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return category;
    }

    public async Task<bool> UpdateAsync(Category category, CancellationToken ct = default)
    {
        _db.Categories.Update(category);
        var rows = await _db.SaveChangesAsync(ct);
        return rows > 0;
    }

    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return false;

        _db.Categories.Remove(entity);
        var rows = await _db.SaveChangesAsync(ct);
        return rows > 0;
    }
}
