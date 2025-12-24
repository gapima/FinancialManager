using FinancialManager.Application.Contracts.Dashboard;

namespace FinancialManager.Application.Abstractions.Repository;

public interface IDashboardRepository
{
    Task<List<TotaisPorPessoaItemDto>> GetTotaisPorPessoaAsync(CancellationToken ct = default);
}
