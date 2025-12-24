using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Domain.Entities;
using FinancialManager.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Repositories;

/*
 * Projeto: FinancialManager
 * Camada: Infrastructure
 * Responsabilidade:
 * - Implementar o contrato ICategoryRepository utilizando Entity Framework Core.
 *
 * Observações de design:
 * - Operações de leitura utilizam AsNoTracking() para melhor performance.
 * - Para Update, é utilizada a estratégia de Update direto da entidade,
 *   assumindo que ela já representa o estado desejado.
 * - Regras de integridade referencial (Restrict/Cascade) são tratadas no DbContext.
 */
public sealed class CategoryRepository : ICategoryRepository
{
    private readonly AppDbContext _db;

    /// <summary>
    /// Constrói o repositório de Category com o DbContext injetado via DI.
    /// </summary>
    /// <param name="db">Contexto EF Core responsável pelo acesso ao banco.</param>
    public CategoryRepository(AppDbContext db) => _db = db;

    /// <summary>
    /// Retorna todas as categorias cadastradas.
    /// </summary>
    /// <remarks>
    /// AsNoTracking(): leitura sem necessidade de rastreamento,
    /// reduzindo overhead de memória.
    /// </remarks>
    public Task<List<Category>> GetAllAsync(CancellationToken ct = default)
        => _db.Categories
            .AsNoTracking()
            .ToListAsync(ct);

    /// <summary>
    /// Retorna uma categoria pelo Id.
    /// </summary>
    /// <remarks>
    /// Leitura sem tracking, pois não há intenção de alteração direta.
    /// </remarks>
    public Task<Category?> GetByIdAsync(int id, CancellationToken ct = default)
        => _db.Categories
            .AsNoTracking()
            .FirstOrDefaultAsync(c => c.Id == id, ct);

    /// <summary>
    /// Adiciona uma nova categoria e persiste no banco.
    /// </summary>
    /// <param name="category">Entidade de domínio já validada.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Entidade persistida (com Id preenchido).</returns>
    public async Task<Category> AddAsync(Category category, CancellationToken ct = default)
    {
        _db.Categories.Add(category);
        await _db.SaveChangesAsync(ct);
        return category;
    }

    /// <summary>
    /// Atualiza uma categoria existente.
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
    public async Task<bool> UpdateAsync(Category category, CancellationToken ct = default)
    {
        _db.Categories.Update(category);
        var rows = await _db.SaveChangesAsync(ct);

        // rows > 0 indica que houve alteração persistida.
        return rows > 0;
    }

    /// <summary>
    /// Remove uma categoria pelo Id.
    /// </summary>
    /// <remarks>
    /// A deleção pode falhar quando:
    /// - a categoria não existe
    /// - existem transações vinculadas (DeleteBehavior.Restrict),
    ///   nesse caso o banco impede a operação.
    /// </remarks>
    public async Task<bool> DeleteAsync(int id, CancellationToken ct = default)
    {
        var entity = await _db.Categories.FirstOrDefaultAsync(c => c.Id == id, ct);
        if (entity is null) return false;

        _db.Categories.Remove(entity);
        var rows = await _db.SaveChangesAsync(ct);
        return rows > 0;
    }
}
