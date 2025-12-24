using FinancialManager.Application.Contracts.Dashboard;

namespace FinancialManager.Application.Abstractions.Service;

/*
 * Projeto: FinancialManager
 * Camada: Application (Abstractions)
 * Responsabilidade:
 * - Definir os casos de uso do Dashboard (consultas agregadas e consolidadas).
 *
 * Observações de design:
 * - O Service retorna DTOs de dashboard (projeções), e não entidades do domínio.
 * - Além dos itens agrupados (por Pessoa/Categoria), o Service consolida o TotalGeral
 *   (TotalReceitas, TotalDespesas e Saldo).
 */
public interface IDashboardService
{
    /// <summary>
    /// Retorna totais agrupados por Pessoa e também o total geral consolidado.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>DTO contendo itens por pessoa e total geral (receitas, despesas e saldo).</returns>
    Task<DashboardTotaisPorPessoaDto> GetTotaisPorPessoaAsync(CancellationToken ct = default);

    /// <summary>
    /// Retorna totais agrupados por Categoria e também o total geral consolidado.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>DTO contendo itens por categoria e total geral (receitas, despesas e saldo).</returns>
    Task<DashboardTotaisPorCategoriaDto> GetTotaisPorCategoriaAsync(CancellationToken ct = default);
}
