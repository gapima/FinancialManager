namespace FinancialManager.Application.Contracts.Pessoa;

public sealed class PessoaCreateDto
{
    public string Nome { get; set; } = string.Empty;
    public int Idade { get; set; }
}
