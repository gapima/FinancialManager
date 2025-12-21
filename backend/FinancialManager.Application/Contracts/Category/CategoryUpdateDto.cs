using FinancialManager.Domain.Enums;

namespace FinancialManager.Application.Contracts.Category;

public sealed class CategoryUpdateDto
{
    public string Description { get; set; } = string.Empty;
    public CategoryPurpose Purpose { get; set; }
}
