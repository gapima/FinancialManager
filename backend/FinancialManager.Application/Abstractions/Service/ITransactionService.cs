using FinancialManager.Application.Contracts.Transaction;

namespace FinancialManager.Application.Abstractions.Service;

/*
 * Projeto: FinancialManager
 * Camada: Application (Abstractions)
 * Responsabilidade:
 * - Definir os casos de uso relacionados à entidade Transaction.
 *
 * Observações de design:
 * - A interface expõe apenas DTOs, mantendo o domínio isolado da API.
 * - A implementação concreta é responsável por:
 *   - validar dados de entrada,
 *   - validar integridade referencial (Pessoa e Category),
 *   - definir CreatedAt no servidor,
 *   - orquestrar persistência via repositório.
 */
public interface ITransactionService
{
    /// <summary>
    /// Retorna todas as transações cadastradas.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de transações no formato de resposta.</returns>
    Task<List<TransactionResponseDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Retorna uma transação pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da transação.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// DTO da transação quando encontrada;
    /// null quando não existe ou id é inválido.
    /// </returns>
    Task<TransactionResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Cria uma nova transação.
    /// </summary>
    /// <param name="dto">Dados necessários para criação.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Transação criada no formato de resposta.</returns>
    Task<TransactionResponseDto> CreateAsync(TransactionCreateDto dto, CancellationToken ct = default);

    /// <summary>
    /// Atualiza uma transação existente.
    /// </summary>
    /// <param name="id">Identificador da transação.</param>
    /// <param name="dto">Dados para atualização.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando atualizou com sucesso;
    /// False quando id é inválido ou o registro não existe.
    /// </returns>
    Task<bool> UpdateAsync(int id, TransactionUpdateDto dto, CancellationToken ct = default);

    /// <summary>
    /// Remove uma transação pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da transação.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando removeu com sucesso;
    /// False quando não foi possível remover.
    /// </returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
