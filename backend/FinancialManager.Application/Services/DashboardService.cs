using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Dashboard;

namespace FinancialManager.Application.Services;

/*
 * Projeto: FinancialManager
 * Camada: Application
 * Responsabilidade:
 * - Orquestrar consultas agregadas para o Dashboard.
 *
 * Observações de design:
 * - A camada Infrastructure (Repository) retorna os itens já agrupados
 *   (por Pessoa / por Categoria), normalmente via SQL/EF com GroupBy.
 * - A camada Application calcula o "TotalGeral" (somatórios e saldo),
 *   centralizando a regra de consolidação em um único ponto.
 * - Isso evita duplicação da regra no front-end e garante consistência de cálculo.
 */
public sealed class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repo;

    /// <summary>
    /// Constrói o serviço de Dashboard.
    /// </summary>
    /// <param name="repo">Repositório responsável por consultas agregadas (somatórios por grupo).</param>
    public DashboardService(IDashboardRepository repo)
    {
        _repo = repo;
    }

    /// <summary>
    /// Retorna totais (receitas, despesas e saldo) agrupados por Pessoa,
    /// além do total geral consolidado.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// Estrutura contendo:
    /// - Items: lista com totais por pessoa
    /// - TotalGeral: soma dos totais e saldo geral (Receitas - Despesas)
    /// </returns>
    public async Task<DashboardTotaisPorPessoaDto> GetTotaisPorPessoaAsync(CancellationToken ct = default)
    {
        // Os itens são obtidos já agregados pelo repositório.
        var items = await _repo.GetTotaisPorPessoaAsync(ct);

        // Consolidação do total geral no servidor para manter consistência
        // (evita que cada client calcule de um jeito).
        var totalReceitas = items.Sum(x => x.TotalReceitas);
        var totalDespesas = items.Sum(x => x.TotalDespesas);

        return new DashboardTotaisPorPessoaDto
        {
            Items = items,
            TotalGeral = new TotaisGeralDto
            {
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                Saldo = totalReceitas - totalDespesas
            }
        };
    }

    /// <summary>
    /// Retorna totais (receitas, despesas e saldo) agrupados por Categoria,
    /// além do total geral consolidado.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// Estrutura contendo:
    /// - Items: lista com totais por categoria
    /// - TotalGeral: soma dos totais e saldo geral (Receitas - Despesas)
    /// </returns>
    public async Task<DashboardTotaisPorCategoriaDto> GetTotaisPorCategoriaAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetTotaisPorCategoriaAsync(ct);

        var totalReceitas = items.Sum(x => x.TotalReceitas);
        var totalDespesas = items.Sum(x => x.TotalDespesas);

        return new DashboardTotaisPorCategoriaDto
        {
            Items = items,
            TotalGeral = new TotaisGeralDto
            {
                TotalReceitas = totalReceitas,
                TotalDespesas = totalDespesas,
                Saldo = totalReceitas - totalDespesas
            }
        };
    }
}
