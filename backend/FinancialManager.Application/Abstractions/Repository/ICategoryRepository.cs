using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Abstractions.Repository;

/*
 * Projeto: FinancialManager
 * Camada: Application (Abstractions)
 * Responsabilidade:
 * - Definir o contrato de persistência da entidade Category.
 *
 * Observações de design:
 * - A interface abstrai completamente a tecnologia de persistência
 *   (EF Core, SQL, etc.).
 * - Trabalha diretamente com entidades de domínio, pois representa
 *   o acesso ao modelo interno do sistema.
 * - Permite substituição da infraestrutura sem impactar a camada Application.
 */
public interface ICategoryRepository
{
    /// <summary>
    /// Retorna todas as categorias persistidas.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de entidades Category.</returns>
    Task<List<Category>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Retorna uma categoria pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// Entidade Category quando encontrada; null quando não existe.
    /// </returns>
    Task<Category?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Persiste uma nova categoria.
    /// </summary>
    /// <param name="category">Entidade de domínio já validada.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Entidade persistida (com identificador gerado).</returns>
    Task<Category> AddAsync(Category category, CancellationToken ct = default);

    /// <summary>
    /// Atualiza uma categoria existente.
    /// </summary>
    /// <param name="category">Entidade com dados atualizados.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando a atualização foi realizada com sucesso;
    /// False quando não foi possível atualizar (ex: registro inexistente).
    /// </returns>
    Task<bool> UpdateAsync(Category category, CancellationToken ct = default);

    /// <summary>
    /// Remove uma categoria pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando removeu com sucesso;
    /// False quando o registro não existe ou não pôde ser removido.
    /// </returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
