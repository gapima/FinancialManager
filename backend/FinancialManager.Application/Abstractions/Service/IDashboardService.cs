using FinancialManager.Application.Contracts.Dashboard;

namespace FinancialManager.Application.Abstractions.Service;

public interface IDashboardService
{
    Task<DashboardTotaisPorPessoaDto> GetTotaisPorPessoaAsync(CancellationToken ct = default);
}
