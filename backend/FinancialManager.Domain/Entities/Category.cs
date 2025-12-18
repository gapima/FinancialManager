using FinancialManager.Domain.Enums;

namespace FinancialManager.Domain.Entities;

public class Category
{
    public int Id { get; set; }
    public string Description { get; set; } = string.Empty;
    public CategoryPurpose Purpose { get; set; }
}
