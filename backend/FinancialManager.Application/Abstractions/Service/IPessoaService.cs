using FinancialManager.Application.Contracts.Pessoa;

namespace FinancialManager.Application.Abstractions.Service;

public interface IPessoaService
{
    Task<List<PessoaResponseDto>> GetAllAsync(CancellationToken ct = default);
    Task<PessoaResponseDto?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<PessoaResponseDto> CreateAsync(PessoaCreateDto dto, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, PessoaUpdateDto dto, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
