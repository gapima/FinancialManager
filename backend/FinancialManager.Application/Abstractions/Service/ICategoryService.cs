using FinancialManager.Application.Contracts.Category;

namespace FinancialManager.Application.Abstractions.Service;

/*
 * Projeto: FinancialManager
 * Camada: Application (Abstractions)
 * Responsabilidade:
 * - Definir os casos de uso relacionados à entidade Category.
 *
 * Observações de design:
 * - Esta interface expõe DTOs (contrato externo) e não entidades de domínio,
 *   protegendo o modelo interno do sistema.
 * - A implementação concreta é responsável por:
 *   - validar dados de entrada,
 *   - orquestrar regras de negócio,
 *   - mapear DTOs para entidades e vice-versa.
 */
public interface ICategoryService
{
    /// <summary>
    /// Retorna todas as categorias cadastradas.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de categorias no formato de resposta.</returns>
    Task<List<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Retorna uma categoria pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// DTO da categoria quando encontrada; null quando não existe ou id é inválido.
    /// </returns>
    Task<CategoryResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Cria uma nova categoria.
    /// </summary>
    /// <param name="dto">Dados necessários para criação.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Categoria criada no formato de resposta.</returns>
    Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default);

    /// <summary>
    /// Atualiza uma categoria existente.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="dto">Dados para atualização.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando atualizou com sucesso;
    /// False quando id é inválido ou o registro não existe.
    /// </returns>
    Task<bool> UpdateAsync(int id, CategoryUpdateDto dto, CancellationToken ct = default);

    /// <summary>
    /// Remove uma categoria pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da categoria.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando removeu com sucesso; False quando não foi possível remover.
    /// </returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
