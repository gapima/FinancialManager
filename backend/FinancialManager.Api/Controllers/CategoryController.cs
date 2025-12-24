using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Category;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManager.Api.Controllers;

/*
 * Projeto: FinancialManager
 * Camada: Api
 * Responsabilidade:
 * - Expor endpoints REST para operações de Category (CRUD).
 *
 * Observações de design:
 * - Controller fino: delega validações e regras para a camada Application (ICategoryService).
 * - Respostas HTTP são traduzidas a partir do resultado do Service:
 *   - null/false -> NotFound (ou BadRequest dependendo da regra)
 *   - sucesso -> Ok / Created / NoContent
 *
 */
[ApiController]
[Route("api/[controller]")]
public sealed class CategoryController : ControllerBase
{
    private readonly ICategoryService _service;

    /// <summary>
    /// Constrói o controller de Category.
    /// </summary>
    /// <param name="service">Serviço de aplicação responsável pelos casos de uso de Category.</param>
    public CategoryController(ICategoryService service)
        => _service = service;

    /// <summary>
    /// Lista todas as categorias cadastradas.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de categorias.</returns>
    /// <response code="200">Retorna a lista de categorias.</response>
    [HttpGet]
    [ProducesResponseType(typeof(List<CategoryResponseDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CategoryResponseDto>>> GetAll(CancellationToken ct)
        => Ok(await _service.GetAllAsync(ct));

    /// <summary>
    /// Busca uma categoria pelo Id.
    /// </summary>
    /// <param name="id">Id da categoria (inteiro &gt; 0).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Categoria quando encontrada.</returns>
    /// <response code="200">Retorna a categoria encontrada.</response>
    /// <response code="404">Quando não existe categoria com o Id informado (ou Id inválido, conforme regra do Service).</response>
    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<CategoryResponseDto>> GetById(int id, CancellationToken ct)
    {
        // O Service retorna null quando:
        // - id inválido (<= 0), ou
        // - entidade não encontrada.
        // O controller traduz isso para 404.
        var dto = await _service.GetByIdAsync(id, ct);
        return dto is null ? NotFound() : Ok(dto);
    }

    /// <summary>
    /// Cria uma nova categoria.
    /// </summary>
    /// <param name="input">Dados da categoria.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Categoria criada.</returns>
    /// <response code="201">Retorna a categoria criada e o Location para consulta.</response>
    /// <response code="400">Quando o payload é inválido (via validação/tratamento global).</response>
    [HttpPost]
    [ProducesResponseType(typeof(CategoryResponseDto), StatusCodes.Status201Created)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<CategoryResponseDto>> Create([FromBody] CategoryCreateDto input, CancellationToken ct)
    {
        // Padrão REST: 201 Created + Location apontando para o recurso recém-criado.
        var created = await _service.CreateAsync(input, ct);
        return CreatedAtAction(nameof(GetById), new { id = created.Id }, created);
    }

    /// <summary>
    /// Atualiza uma categoria existente.
    /// </summary>
    /// <param name="id">Id da categoria.</param>
    /// <param name="input">Dados para atualização.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <response code="204">Atualização realizada com sucesso (sem corpo).</response>
    /// <response code="404">Categoria não encontrada (ou Id inválido).</response>
    /// <response code="400">Quando o payload é inválido (via validação/tratamento global).</response>
    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Update(int id, [FromBody] CategoryUpdateDto input, CancellationToken ct)
    {
        var ok = await _service.UpdateAsync(id, input, ct);
        return ok ? NoContent() : NotFound();
    }

    /// <summary>
    /// Remove uma categoria pelo Id.
    /// </summary>
    /// <param name="id">Id da categoria.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <response code="204">Remoção realizada com sucesso (sem corpo).</response>
    /// <response code="404">Categoria não encontrada (ou Id inválido).</response>
    /// <response code="400">
    /// Quando a categoria não pode ser removida por integridade referencial
    /// (ex.: existem Transactions vinculadas e o banco está com Restrict).
    /// </response>
    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(StatusCodes.Status400BadRequest)]
    public async Task<IActionResult> Delete(int id, CancellationToken ct)
    {
        // Se o DB estiver com Restrict e houver Transactions vinculadas,
        // o EF pode lançar exceção ao salvar. O ideal é um tratamento global
        // para traduzir isso para 400 (ou 409 Conflict).
        var ok = await _service.DeleteAsync(id, ct);
        return ok ? NoContent() : NotFound();
    }
}
