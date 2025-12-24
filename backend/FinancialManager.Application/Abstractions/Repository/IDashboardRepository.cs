using FinancialManager.Application.Contracts.Dashboard;

namespace FinancialManager.Application.Abstractions.Repository;

/*
 * Projeto: FinancialManager
 * Camada: Application (Abstractions)
 * Responsabilidade:
 * - Definir o contrato de consultas agregadas para o Dashboard.
 *
 * Observações de design:
 * - Este repositório retorna DTOs específicos de dashboard (não entidades),
 *   pois o objetivo é fornecer projeções agregadas (somatórios) e não o modelo completo.
 * - A implementação concreta (Infrastructure) pode utilizar EF Core/SQL com GroupBy e joins
 *   para calcular totais por Pessoa/Categoria de forma eficiente.
 */
public interface IDashboardRepository
{
    /// <summary>
    /// Retorna totais agrupados por Pessoa (receitas e despesas).
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// Lista de itens contendo a identificação da pessoa e os totais agregados
    /// (TotalReceitas e TotalDespesas).
    /// </returns>
    Task<List<TotaisPorPessoaItemDto>> GetTotaisPorPessoaAsync(CancellationToken ct = default);

    /// <summary>
    /// Retorna totais agrupados por Categoria (receitas e despesas).
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// Lista de itens contendo a identificação da categoria e os totais agregados
    /// (TotalReceitas e TotalDespesas).
    /// </returns>
    Task<List<TotaisPorCategoriaItemDto>> GetTotaisPorCategoriaAsync(CancellationToken ct = default);
}
