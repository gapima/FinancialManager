using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManager.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    [HttpGet("totais-por-pessoa")]
    public async Task<ActionResult<DashboardTotaisPorPessoaDto>> GetTotaisPorPessoa(CancellationToken ct)
    {
        var dto = await _service.GetTotaisPorPessoaAsync(ct);
        return Ok(dto);
    }
}
