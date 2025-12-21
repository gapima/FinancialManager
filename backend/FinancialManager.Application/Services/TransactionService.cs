using AutoMapper;
using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Transaction;
using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Services;

public sealed class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repo;
    private readonly IMapper _mapper;

    public TransactionService(ITransactionRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    public async Task<List<TransactionResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<TransactionResponseDto>>(list);
    }

    public async Task<TransactionResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0) return null;

        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<TransactionResponseDto>(entity);
    }

    public async Task<TransactionResponseDto> CreateAsync(TransactionCreateDto dto, CancellationToken ct = default)
    {
        Validate(dto);

        await ValidateForeignKeys(dto.CategoryId, dto.PessoaId, ct);

        var entity = _mapper.Map<Transaction>(dto);
        entity.CreatedAt = DateTime.UtcNow;

        var created = await _repo.AddAsync(entity, ct);

        return _mapper.Map<TransactionResponseDto>(created);
    }

    public async Task<bool> UpdateAsync(int id, TransactionUpdateDto dto, CancellationToken ct = default)
    {
        if (id <= 0) return false;

        Validate(dto);

        await ValidateForeignKeys(dto.CategoryId, dto.PessoaId, ct);

        // pega “tracked” pra atualizar (ou pega e depois UpdateAsync faz Update)
        var current = await _repo.GetByIdAsync(id, ct);
        if (current is null) return false;

        // como o GetByIdAsync está AsNoTracking no repo, vamos criar uma entidade nova:
        var entity = _mapper.Map<Transaction>(dto);
        entity.Id = id;
        entity.CreatedAt = current.CreatedAt; // preserva

        return await _repo.UpdateAsync(entity, ct);
    }

    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => id <= 0 ? Task.FromResult(false) : _repo.DeleteAsync(id, ct);

    private static void Validate(TransactionCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new ArgumentException("Description é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Amount))
            throw new ArgumentException("Amount é obrigatório.");

        if (dto.CategoryId <= 0)
            throw new ArgumentException("CategoryId inválido.");

        if (dto.PessoaId <= 0)
            throw new ArgumentException("PessoaId inválido.");
    }

    private static void Validate(TransactionUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new ArgumentException("Description é obrigatório.");

        if (string.IsNullOrWhiteSpace(dto.Amount))
            throw new ArgumentException("Amount é obrigatório.");

        if (dto.CategoryId <= 0)
            throw new ArgumentException("CategoryId inválido.");

        if (dto.PessoaId <= 0)
            throw new ArgumentException("PessoaId inválido.");
    }

    private async Task ValidateForeignKeys(int categoryId, int pessoaId, CancellationToken ct)
    {
        if (!await _repo.CategoryExistsAsync(categoryId, ct))
            throw new ArgumentException("CategoryId não existe.");

        if (!await _repo.PessoaExistsAsync(pessoaId, ct))
            throw new ArgumentException("PessoaId não existe.");
    }
}
