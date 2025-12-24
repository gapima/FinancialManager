using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Dashboard;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManager.Api.Controllers;

/*
 * Projeto: FinancialManager
 * Camada: Api
 * Responsabilidade:
 * - Expor endpoints de leitura para o Dashboard (consultas agregadas).
 *
 * Observações de design:
 * - O Dashboard é somente leitura (read-only).
 * - Toda a regra de agregação e consolidação fica no back-end,
 *   evitando cálculos duplicados ou inconsistentes no front-end.
 * - O controller é fino e delega completamente ao IDashboardService.
 */
[ApiController]
[Route("api/[controller]")]
public sealed class DashboardController : ControllerBase
{
    private readonly IDashboardService _service;

    /// <summary>
    /// Constrói o controller de Dashboard.
    /// </summary>
    /// <param name="service">Serviço responsável pelas consultas agregadas do dashboard.</param>
    public DashboardController(IDashboardService service)
    {
        _service = service;
    }

    /// <summary>
    /// Retorna totais agrupados por Pessoa.
    /// </summary>
    /// <remarks>
    /// O retorno inclui:
    /// - Lista de pessoas com seus totais de receitas, despesas e saldo
    /// - Total geral consolidado (receitas, despesas e saldo)
    /// </remarks>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Estrutura completa de dashboard por pessoa.</returns>
    /// <response code="200">Retorna os totais agrupados por pessoa.</response>
    [HttpGet("totais-por-pessoa")]
    [ProducesResponseType(typeof(DashboardTotaisPorPessoaDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardTotaisPorPessoaDto>> GetTotaisPorPessoa(CancellationToken ct)
    {
        var dto = await _service.GetTotaisPorPessoaAsync(ct);
        return Ok(dto);
    }

    /// <summary>
    /// Retorna totais agrupados por Categoria.
    /// </summary>
    /// <remarks>
    /// O retorno inclui:
    /// - Lista de categorias com seus totais de receitas, despesas e saldo
    /// - Total geral consolidado (receitas, despesas e saldo)
    /// </remarks>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Estrutura completa de dashboard por categoria.</returns>
    /// <response code="200">Retorna os totais agrupados por categoria.</response>
    [HttpGet("totais-por-categoria")]
    [ProducesResponseType(typeof(DashboardTotaisPorCategoriaDto), StatusCodes.Status200OK)]
    public async Task<ActionResult<DashboardTotaisPorCategoriaDto>> GetTotaisPorCategoria(CancellationToken ct)
    {
        var dto = await _service.GetTotaisPorCategoriaAsync(ct);
        return Ok(dto);
    }
}
