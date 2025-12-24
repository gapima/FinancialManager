using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Transaction;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManager.Api.Controllers;

/*
 * Projeto: FinancialManager
 * Camada: Api
 * Responsabilidade:
 * - Expor endpoints REST para operações de Transaction (CRUD).
 *
 * Observações de design:
 * - Controller fino: delega regras e validações para a camada Application (ITransactionService).
 * - A camada Application:
 *   - valida campos obrigatórios,
 *   - valida integridade referencial (PessoaId e CategoryId),
 *   - define CreatedAt no servidor (UTC).
 *
 */
[ApiController]
[Route("api/[controller]")]
public sealed class TransactionController : ControllerBase
{
    private readonly ITransactionService _service;

    /// <summary>
    /// Constrói o controller de Transaction.
    /// </summary>
    /// <param name="service">Serviço de aplicação responsável pelos casos de uso de Transaction.</param>
    public TransactionController(ITransactionService service)
    {
        _service = service;
    }

    /// <summary>
    /// Lista todas as transações cadastradas.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de transações.</returns>
    /// <response code="200">Retorna a lista de transações.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<TransactionResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TransactionResponseDto>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    /// <summary>
    /// Busca uma transação pelo Id.
    /// </summary>
    /// <param name="id">Id da transação (inteiro &gt; 0).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Transação quando encontrada.</returns>
    /// <response code="200">Retorna a transação encontrada.</response>
    /// <response code="404">Quando não existe transação com o Id informado (ou Id inválido, conforme regra do Service).</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TransactionResponseDto>> GetById(int id, CancellationToken ct)
    {
        // O Service retorna null quando:
        // - id inválido (<= 0), ou
        // - entidade não encontrada.
        // O controller traduz isso para 404.
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>
    /// Cria uma nova transação.
    /// </summary>
    /// <param name="input">Dados da transação.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Transação criada.</returns>
    /// <response code="201">Retorna a transação criada e o Location para consulta.</response>
    /// <response code="400">Quando payload/FKs são inválidos (via validação/tratamento global).</response>
    [HttpPost]
    [ProducesResponseType(typeof(TransactionResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<TransactionResponseDto>> Create([FromBody] TransactionCreateDto input, CancellationToken ct)
    {
        // CreatedAt é definido na camada Application (servidor) para manter consistência de auditoria.
        var created = await _service.CreateAsync(input, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza uma transação existente.
    /// </summary>
    /// <param name="id">Id da transação.</param>
    /// <param name="input">Dados para atualização.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <response code="204">Atualização realizada com sucesso (sem corpo).</response>
    /// <response code="404">Transação não encontrada (ou Id inválido).</response>
    /// <response code="400">Quando payload/FKs são inválidos (via validação/tratamento global).</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] TransactionUpdateDto input, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, input, ct);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>
    /// Remove uma transação pelo Id.
    /// </summary>
    /// <param name="id">Id da transação.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <response code="204">Remoção realizada com sucesso (sem corpo).</response>
    /// <response code="404">Transação não encontrada (ou Id inválido).</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
