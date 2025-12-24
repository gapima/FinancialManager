using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Dashboard;

namespace FinancialManager.Application.Services;

public sealed class DashboardService : IDashboardService
{
    private readonly IDashboardRepository _repo;

    public DashboardService(IDashboardRepository repo)
    {
        _repo = repo;
    }

    public async Task<DashboardTotaisPorPessoaDto> GetTotaisPorPessoaAsync(CancellationToken ct = default)
    {
        var items = await _repo.GetTotaisPorPessoaAsync(ct);

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
}
