namespace FinancialManager.Application.Contracts.Dashboard;

public sealed class DashboardTotaisPorCategoriaDto
{
    public List<TotaisPorCategoriaItemDto> Items { get; set; } = new();
    public TotaisGeralDto TotalGeral { get; set; } = new();
}

public sealed class TotaisPorCategoriaItemDto
{
    public int CategoryId { get; set; }
    public string CategoryDescription { get; set; } = string.Empty;

    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo { get; set; }
}
