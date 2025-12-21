using AutoMapper;
using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Pessoa;
using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Services;

public sealed class PessoaService : IPessoaService
{
    private readonly IPessoaRepository _repo;
    private readonly IMapper _mapper;

    public PessoaService(IPessoaRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<PessoaResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var pessoas = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<PessoaResponseDto>>(pessoas);
    }

    public async Task<PessoaResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0) return null;

        var pessoa = await _repo.GetByIdAsync(id, ct);
        return pessoa is null ? null : _mapper.Map<PessoaResponseDto>(pessoa);
    }

    public async Task<PessoaResponseDto> CreateAsync(PessoaCreateDto dto, CancellationToken ct = default)
    {
        Validate(dto.Nome, dto.Idade);

        var pessoa = _mapper.Map<Pessoa>(dto);

        var created = await _repo.AddAsync(pessoa, ct);

        return _mapper.Map<PessoaResponseDto>(created);
    }

    public async Task<bool> UpdateAsync(int id, PessoaUpdateDto dto, CancellationToken ct = default)
    {
        if (id <= 0) return false;

        Validate(dto.Nome, dto.Idade);

        var pessoa = await _repo.GetByIdAsync(id, ct);
        if (pessoa is null) return false;

        _mapper.Map(dto, pessoa);

        pessoa.Id = id;

        return await _repo.UpdateAsync(pessoa, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => id <= 0 ? Task.FromResult(false) : _repo.DeleteAsync(id, ct);

    private static void Validate(string nome, int idade)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.");

        if (idade < 0)
            throw new ArgumentException("Idade inválida.");
    }
}
