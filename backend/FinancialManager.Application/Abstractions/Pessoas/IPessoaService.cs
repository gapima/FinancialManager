using FinancialManager.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FinancialManager.Application.Abstractions.Pessoas;

public interface IPessoaService
{
    Task<List<Pessoa>> GetAllAsync(CancellationToken ct = default);
    Task<Pessoa?> GetByIdAsync(int id, CancellationToken ct = default);
    Task<Pessoa> CreateAsync(Pessoa pessoa, CancellationToken ct = default);
    Task<bool> UpdateAsync(int id, Pessoa pessoa, CancellationToken ct = default);
    Task<bool> DeleteAsync(int id, CancellationToken ct = default);
}
