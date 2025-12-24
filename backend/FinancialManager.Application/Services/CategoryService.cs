using AutoMapper;
using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Category;
using FinancialManager.Domain.Entities;
using FinancialManager.Domain.Enums;

namespace FinancialManager.Application.Services;

/*
 * Projeto: FinancialManager
 * Camada: Application
 * Responsabilidade:
 * - Implementar os casos de uso relacionados à entidade Category (CRUD).
 *
 * Observações de design:
 * - A camada Application orquestra o fluxo: valida entrada, chama repositório e mapeia DTOs.
 * - O serviço expõe DTOs (contrato externo) e protege o domínio de acoplamento com a API.
 * - A validação do enum CategoryPurpose é feita aqui para garantir consistência antes de persistir.
 */
public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constrói o serviço de Category.
    /// </summary>
    /// <param name="repo">Abstração de persistência de Category (implementada na Infrastructure).</param>
    /// <param name="mapper">Responsável por mapear DTOs &lt;-&gt; Entidades.</param>
    public CategoryService(ICategoryRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    /// <summary>
    /// Retorna todas as categorias cadastradas.
    /// </summary>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Lista de categorias no formato de resposta (DTO).</returns>
    public async Task<List<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<CategoryResponseDto>>(categories);
    }

    /// <summary>
    /// Retorna uma categoria pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da categoria (deve ser &gt; 0).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// DTO da categoria, ou null quando o id é inválido ou o registro não existe.
    /// </returns>
    public async Task<CategoryResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        // Regra: ids não positivos são inválidos.
        // Retornar null simplifica o fluxo (Controller decide o status HTTP).
        if (id <= 0) return null;

        var category = await _repo.GetByIdAsync(id, ct);
        return category is null ? null : _mapper.Map<CategoryResponseDto>(category);
    }

    /// <summary>
    /// Cria uma nova categoria.
    /// </summary>
    /// <param name="dto">Dados necessários para criação.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Categoria criada no formato de resposta (DTO).</returns>
    /// <exception cref="ArgumentException">Quando os dados de entrada são inválidos.</exception>
    public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default)
    {
        // Valida antes de persistir: garante consistência do domínio e evita registros inválidos.
        Validate(dto.Description, dto.Purpose);

        var category = _mapper.Map<Category>(dto);
        var created = await _repo.AddAsync(category, ct);

        return _mapper.Map<CategoryResponseDto>(created);
    }

    /// <summary>
    /// Atualiza uma categoria existente.
    /// </summary>
    /// <param name="id">Identificador da categoria (deve ser &gt; 0).</param>
    /// <param name="dto">Dados para atualização.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando atualizou com sucesso; False quando id é inválido ou não encontrou o registro.
    /// </returns>
    /// <exception cref="ArgumentException">Quando os dados de entrada são inválidos.</exception>
    public async Task<bool> UpdateAsync(int id, CategoryUpdateDto dto, CancellationToken ct = default)
    {
        if (id <= 0) return false;

        Validate(dto.Description, dto.Purpose);

        // Busca antes de atualizar para garantir existência e evitar update "cego".
        var category = await _repo.GetByIdAsync(id, ct);
        if (category is null) return false;

        // Aplica alterações do DTO na entidade existente.
        _mapper.Map(dto, category);

        // Garantia: id do path tem precedência sobre qualquer valor do DTO.
        category.Id = id;

        return await _repo.UpdateAsync(category, ct);
    }

    /// <summary>
    /// Remove uma categoria pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da categoria (deve ser &gt; 0).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>True se removeu; False se id inválido ou não removeu.</returns>
    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => id <= 0 ? Task.FromResult(false) : _repo.DeleteAsync(id, ct);

    /// <summary>
    /// Valida regras mínimas de consistência para Category.
    /// </summary>
    /// <param name="description">Descrição da categoria (obrigatória).</param>
    /// <param name="purpose">Finalidade/tipo da categoria (enum válido).</param>
    /// <exception cref="ArgumentException">Quando as regras de consistência falham.</exception>
    private static void Validate(string description, CategoryPurpose purpose)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description é obrigatória.");

        // Como Purpose é um enum do Domain, garantimos que o valor recebido é válido.
        // Isso evita persistir valores fora do esperado e facilita evolução do domínio.
        if (!Enum.IsDefined(typeof(CategoryPurpose), purpose))
            throw new ArgumentException("Purpose inválido.");
    }
}
