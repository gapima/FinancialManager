using AutoMapper;
using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Category;
using FinancialManager.Domain.Entities;
using FinancialManager.Domain.Enums;

namespace FinancialManager.Application.Services;

public sealed class CategoryService : ICategoryService
{
    private readonly ICategoryRepository _repo;
    private readonly IMapper _mapper;

    public CategoryService(ICategoryRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var categories = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<CategoryResponseDto>>(categories);
    }

    public async Task<CategoryResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0) return null;

        var category = await _repo.GetByIdAsync(id, ct);
        return category is null ? null : _mapper.Map<CategoryResponseDto>(category);
    }

    public async Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default)
    {
        Validate(dto.Description, dto.Purpose);

        var category = _mapper.Map<Category>(dto);
        var created = await _repo.AddAsync(category, ct);

        return _mapper.Map<CategoryResponseDto>(created);
    }

    public async Task<bool> UpdateAsync(int id, CategoryUpdateDto dto, CancellationToken ct = default)
    {
        if (id <= 0) return false;

        Validate(dto.Description, dto.Purpose);

        var category = await _repo.GetByIdAsync(id, ct);
        if (category is null) return false;

        _mapper.Map(dto, category);
        category.Id = id;

        return await _repo.UpdateAsync(category, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => id <= 0 ? Task.FromResult(false) : _repo.DeleteAsync(id, ct);

    private static void Validate(string description, CategoryPurpose purpose)
    {
        if (string.IsNullOrWhiteSpace(description))
            throw new ArgumentException("Description é obrigatória.");

        // se você usa enum no Domain, valida aqui
        if (!Enum.IsDefined(typeof(CategoryPurpose), purpose))
            throw new ArgumentException("Purpose inválido.");
    }
}
