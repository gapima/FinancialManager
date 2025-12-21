using FinancialManager.Domain.Enums;

namespace FinancialManager.Application.Contracts.Category;

public sealed class CategoryCreateDto
{
    public string Description { get; set; } = string.Empty;
    public CategoryPurpose Purpose { get; set; }
}
