using FinancialManager.Application.Abstractions.Pessoas;
using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Services;

public sealed class PessoaService : IPessoaService
{
    private readonly IPessoaRepository _repo;

    public PessoaService(IPessoaRepository repo)
    {
        _repo = repo;
    }

    public Task<List<Pessoa>> GetAllAsync(CancellationToken ct = default)
        => _repo.GetAllAsync(ct);

    public Task<Pessoa?> GetByIdAsync(int id, CancellationToken ct = default)
        => _repo.GetByIdAsync(id, ct);

    public Task<Pessoa> CreateAsync(Pessoa pessoa, CancellationToken ct = default)
    {
        // Aqui você pluga validações/regras no futuro
        if (string.IsNullOrWhiteSpace(pessoa.Nome))
            throw new ArgumentException("Nome é obrigatório.");

        if (pessoa.Idade < 0)
            throw new ArgumentException("Idade inválida.");

        return _repo.AddAsync(pessoa, ct);
    }

    public async Task<bool> UpdateAsync(int id, Pessoa pessoa, CancellationToken ct = default)
    {
        if (id <= 0) return false;

        // força consistência do id
        pessoa.Id = id;

        if (string.IsNullOrWhiteSpace(pessoa.Nome))
            throw new ArgumentException("Nome é obrigatório.");

        if (pessoa.Idade < 0)
            throw new ArgumentException("Idade inválida.");

        return await _repo.UpdateAsync(pessoa, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => _repo.DeleteAsync(id, ct);
}
