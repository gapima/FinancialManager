using FinancialManager.Application.Contracts.Category;

namespace FinancialManager.Application.Abstractions.Service;

public interface ICategoryService
{
    Task<List<CategoryResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task<CategoryResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<CategoryResponseDto> CreateAsync(CategoryCreateDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, CategoryUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
