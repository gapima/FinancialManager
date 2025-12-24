using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Abstractions.Repository;

/*
 * Projeto: FinancialManager
 * Camada: Application (Abstractions)
 * Responsabilidade:
 * - Definir o contrato de persistência da entidade Pessoa.
 *
 * Observações de design:
 * - Esta interface NÃO contém detalhes de implementação (EF, SQL, etc.).
 * - Permite trocar a infraestrutura de dados sem impactar a camada Application.
 * - Trabalha diretamente com entidades de domínio, pois representa acesso ao modelo interno.
 */
public interface IPessoaRepository
{
    /// <summary>
    /// Retorna todas as pessoas persistidas.
    /// </summary>
    /// <param name="ct">Token de cancelamento para operações longas.</param>
    /// <returns>Lista de entidades Pessoa.</returns>
    Task<List<Pessoa>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Retorna uma pessoa pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da pessoa.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// Entidade Pessoa quando encontrada; null quando não existe.
    /// </returns>
    Task<Pessoa?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Persiste uma nova pessoa.
    /// </summary>
    /// <param name="pessoa">Entidade de domínio já validada.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Entidade persistida (com identificador gerado).</returns>
    Task<Pessoa> AddAsync(Pessoa pessoa, CancellationToken ct = default);

    /// <summary>
    /// Atualiza uma pessoa existente.
    /// </summary>
    /// <param name="pessoa">Entidade com dados atualizados.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando a atualização foi realizada com sucesso;
    /// False quando não foi possível atualizar (ex: registro inexistente).
    /// </returns>
    Task<bool> UpdateAsync(Pessoa pessoa, CancellationToken ct = default);

    /// <summary>
    /// Remove uma pessoa pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da pessoa.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando removeu com sucesso;
    /// False quando o registro não existe ou não pôde ser removido.
    /// </returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
