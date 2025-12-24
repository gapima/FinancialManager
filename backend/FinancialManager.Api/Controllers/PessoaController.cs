using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Pessoa;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManager.Api.Controllers;

/*
 * Projeto: FinancialManager
 * Camada: Api
 * Responsabilidade:
 * - Expor endpoints REST para operações de Pessoa (CRUD).
 *
 * Observações de design:
 * - O Controller é fino: delega regra de negócio para a camada Application (IPessoaService).
 * - O retorno HTTP é montado com base no resultado do Service:
 *   - null/false -> NotFound (ou BadRequest dependendo da regra)
 *   - sucesso -> Ok / Created / NoContent
 */
[ApiController]
[Route("api/[controller]")]
public class PessoaController : ControllerBase
{
    private readonly IPessoaService _service;

    /// <summary>
    /// Constrói o controller de Pessoa.
    /// </summary>
    /// <param name="service">Serviço de aplicação responsável pelos casos de uso de Pessoa.</param>
    public PessoaController(IPessoaService service)
        => _service = service;

    /// <summary>
    /// Lista todas as pessoas cadastradas.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de pessoas.</returns>
    /// <response code="200">Retorna a lista de pessoas.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<PessoaResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<PessoaResponseDto>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    /// <summary>
    /// Busca uma pessoa pelo Id.
    /// </summary>
    /// <param name="id">Id da pessoa (inteiro &gt; 0).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Uma pessoa quando encontrada.</returns>
    /// <response code="200">Retorna a pessoa encontrada.</response>
    /// <response code="404">Quando não existe pessoa com o Id informado (ou Id inválido, conforme regra do Service).</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(PessoaResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<PessoaResponseDto>> GetById(int id, CancellationToken ct)
    {
        // O Service retorna null quando:
        // - id inválido (<= 0), ou
        // - entidade não encontrada.
        // O controller traduz isso para HTTP 404.
        var pessoa = await _service.GetByIdAsync(id, ct);
        return pessoa is null ? NotFound() : Ok(pessoa);
    }

    /// <summary>
    /// Cria uma nova pessoa.
    /// </summary>
    /// <param name="input">Dados da pessoa.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>A pessoa criada.</returns>
    /// <response code="201">Retorna a pessoa criada e o Location para consulta.</response>
    /// <response code="400">Quando o payload é inválido (validações de negócio/disponíveis via tratamento global).</response>
    [HttpPost]
    [ProducesResponseType(typeof(PessoaResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<PessoaResponseDto>> Create([FromBody] PessoaCreateDto input, CancellationToken ct)
    {
        // CreatedAtAction: padrão REST para retornar 201 + Location do recurso criado.
        var created = await _service.CreateAsync(input, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza uma pessoa existente.
    /// </summary>
    /// <param name="id">Id da pessoa.</param>
    /// <param name="input">Dados para atualização.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <response code="204">Atualização realizada com sucesso (sem corpo).</response>
    /// <response code="404">Pessoa não encontrada (ou Id inválido conforme regra do Service).</response>
    /// <response code="400">Quando o payload é inválido (validações de negócio/disponíveis via tratamento global).</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] PessoaUpdateDto input, CancellationToken ct)
        => await _service.UpdateAsync(id, input, ct) ? NoContent() : NotFound();

    /// <summary>
    /// Remove uma pessoa pelo Id.
    /// </summary>
    /// <param name="id">Id da pessoa.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <response code="204">Remoção realizada com sucesso (sem corpo).</response>
    /// <response code="404">Pessoa não encontrada (ou Id inválido).</response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
        => await _service.DeleteAsync(id, ct) ? NoContent() : NotFound();
}
