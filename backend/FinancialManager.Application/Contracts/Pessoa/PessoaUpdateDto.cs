namespace FinancialManager.Application.Contracts.Pessoa;

public sealed class PessoaUpdateDto
{
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
}
