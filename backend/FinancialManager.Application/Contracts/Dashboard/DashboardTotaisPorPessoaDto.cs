namespace FinancialManager.Application.Contracts.Dashboard;

public sealed class DashboardTotaisPorPessoaDto
{
    public List<TotaisPorPessoaItemDto> Items { get; set; } = new();
    public TotaisGeralDto TotalGeral { get; set; } = new();
}

public sealed class TotaisPorPessoaItemDto
{
    public int PessoaId { get; set; }
    public string PessoaNome { get; set; } = string.Empty;

    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo { get; set; }
}

public sealed class TotaisGeralDto
{
    public decimal TotalReceitas { get; set; }
    public decimal TotalDespesas { get; set; }
    public decimal Saldo { get; set; }
}
