using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Abstractions.Repository;

/*
 * Projeto: FinancialManager
 * Camada: Application (Abstractions)
 * Responsabilidade:
 * - Definir o contrato de persistência da entidade Transaction.
 * - Expor métodos auxiliares para validação de integridade referencial (FKs).
 *
 * Observações de design:
 * - A interface abstrai completamente a tecnologia de persistência (EF Core, SQL, etc.).
 * - Trabalha diretamente com entidades de domínio, pois representa acesso ao modelo interno.
 * - Os métodos CategoryExistsAsync e PessoaExistsAsync permitem:
 *   - validação antecipada de FKs na camada Application
 *   - mensagens de erro mais claras do que exceções genéricas do banco
 */
public interface ITransactionRepository
{
    /// <summary>
    /// Retorna todas as transações persistidas.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de entidades Transaction.</returns>
    Task<List<Transaction>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Retorna uma transação pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da transação.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// Entidade Transaction quando encontrada; null quando não existe.
    /// </returns>
    Task<Transaction?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Persiste uma nova transação.
    /// </summary>
    /// <param name="transaction">Entidade de domínio já validada.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Entidade persistida (com identificador gerado).</returns>
    Task<Transaction> AddAsync(Transaction transaction, CancellationToken ct = default);

    /// <summary>
    /// Atualiza uma transação existente.
    /// </summary>
    /// <param name="transaction">Entidade com dados atualizados.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando a atualização foi realizada com sucesso;
    /// False quando não foi possível atualizar (ex: registro inexistente).
    /// </returns>
    Task<bool> UpdateAsync(Transaction transaction, CancellationToken ct = default);

    /// <summary>
    /// Remove uma transação pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da transação.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando removeu com sucesso;
    /// False quando o registro não existe ou não pôde ser removido.
    /// </returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Verifica se uma Category existe.
    /// </summary>
    /// <remarks>
    /// Método auxiliar para validação antecipada de FK na camada Application,
    /// evitando exceções genéricas do banco de dados.
    /// </remarks>
    Task<bool> CategoryExistsAsync(int categoryId, CancellationToken ct = default);

    /// <summary>
    /// Verifica se uma Pessoa existe.
    /// </summary>
    /// <remarks>
    /// Método auxiliar para validação antecipada de FK na camada Application,
    /// evitando exceções genéricas do banco de dados.
    /// </remarks>
    Task<bool> PessoaExistsAsync(int pessoaId, CancellationToken ct = default);
}
