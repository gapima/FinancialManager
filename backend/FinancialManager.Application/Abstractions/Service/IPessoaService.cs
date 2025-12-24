using FinancialManager.Application.Contracts.Pessoa;

namespace FinancialManager.Application.Abstractions.Service;

/*
 * Projeto: FinancialManager
 * Camada: Application (Abstractions)
 * Responsabilidade:
 * - Definir os casos de uso relacionados à entidade Pessoa.
 *
 * Observações de design:
 * - Esta interface expõe DTOs, e não entidades de domínio,
 *   protegendo o modelo interno do sistema.
 * - A implementação é responsável por validações,
 *   orquestração de regras e mapeamentos.
 */
public interface IPessoaService
{
    /// <summary>
    /// Retorna todas as pessoas cadastradas.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de pessoas no formato de resposta.</returns>
    Task<List<PessoaResponseDto>> GetAllAsync(CancellationToken ct = default);

    /// <summary>
    /// Retorna uma pessoa pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da pessoa.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// DTO da pessoa quando encontrada; null quando não existe ou id é inválido.
    /// </returns>
    Task<PessoaResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);

    /// <summary>
    /// Cria uma nova pessoa.
    /// </summary>
    /// <param name="dto">Dados necessários para criação.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Pessoa criada no formato de resposta.</returns>
    Task<PessoaResponseDto> CreateAsync(PessoaCreateDto dto, CancellationToken ct = default);

    /// <summary>
    /// Atualiza uma pessoa existente.
    /// </summary>
    /// <param name="id">Identificador da pessoa.</param>
    /// <param name="dto">Dados para atualização.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando atualizou com sucesso;
    /// False quando id é inválido ou registro não existe.
    /// </returns>
    Task<bool> UpdateAsync(int id, PessoaUpdateDto dto, CancellationToken ct = default);

    /// <summary>
    /// Remove uma pessoa pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da pessoa.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando removeu com sucesso; False quando não foi possível remover.
    /// </returns>
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
