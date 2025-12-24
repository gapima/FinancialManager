using AutoMapper;
using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Transaction;
using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Services;

/*
 * Projeto: FinancialManager
 * Camada: Application
 * Responsabilidade:
 * - Implementar os casos de uso relacionados à entidade Transaction (CRUD).
 *
 * Observações de design:
 * - A camada Application valida entrada, valida integridade (FKs) e orquestra persistência via repositório.
 * - CreatedAt é definido no servidor (UTC) para evitar divergência de relógio entre clientes.
 * - No Update, preservamos CreatedAt: atualizações não devem alterar a data de criação histórica.
 * - Como o repositório usa AsNoTracking em GetById, o Update é feito com uma entidade "nova"
 *   (detached) mapeada do DTO e persistida via UpdateAsync no repositório.
 */
public sealed class TransactionService : ITransactionService
{
    private readonly ITransactionRepository _repo;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constrói o serviço de Transaction.
    /// </summary>
    /// <param name="repo">Contrato de persistência e validações auxiliares (FK existence).</param>
    /// <param name="mapper">Mapeamento DTO &lt;-&gt; Entidade.</param>
    public TransactionService(ITransactionRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    /// <summary>
    /// Retorna todas as transações cadastradas.
    /// </summary>
    public async Task<List<TransactionResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        var list = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<TransactionResponseDto>>(list);
    }

    /// <summary>
    /// Retorna uma transação pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da transação (deve ser &gt; 0).</param>
    /// <returns>DTO da transação ou null quando não encontrada (ou id inválido).</returns>
    public async Task<TransactionResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        if (id <= 0) return null;

        var entity = await _repo.GetByIdAsync(id, ct);
        return entity is null ? null : _mapper.Map<TransactionResponseDto>(entity);
    }

    /// <summary>
    /// Cria uma nova transação.
    /// </summary>
    /// <remarks>
    /// Regras:
    /// - Valida campos obrigatórios.
    /// - Valida existência de Pessoa e Category antes de persistir (evita erro de FK e melhora mensagem).
    /// - Define CreatedAt no servidor em UTC.
    /// </remarks>
    /// <exception cref="ArgumentException">Quando dados são inválidos ou FKs não existem.</exception>
    public async Task<TransactionResponseDto> CreateAsync(TransactionCreateDto dto, CancellationToken ct = default)
    {
        Validate(dto);

        // Validação "amigável": checa FKs aqui para retornar erro claro, antes do banco reclamar.
        await ValidateForeignKeys(dto.CategoryId, dto.PessoaId, ct);

        var entity = _mapper.Map<Transaction>(dto);

        // Regra: CreatedAt é controlado pelo servidor (UTC).
        entity.CreatedAt = DateTime.UtcNow;

        var created = await _repo.AddAsync(entity, ct);

        return _mapper.Map<TransactionResponseDto>(created);
    }

    /// <summary>
    /// Atualiza uma transação existente.
    /// </summary>
    /// <remarks>
    /// Regras:
    /// - Valida entrada e existência de FKs.
    /// - Preserva CreatedAt (data de criação não deve mudar em updates).
    ///
    /// Nota técnica:
    /// - Como GetByIdAsync no repositório pode ser AsNoTracking(),
    ///   o objeto retornado pode não estar rastreado pelo EF.
    ///   Aqui criamos uma entidade nova a partir do DTO e persistimos via UpdateAsync.
    /// </remarks>
    public async Task<bool> UpdateAsync(int id, TransactionUpdateDto dto, CancellationToken ct = default)
    {
        if (id <= 0) return false;

        Validate(dto);

        await ValidateForeignKeys(dto.CategoryId, dto.PessoaId, ct);

        // Precisamos ler o registro atual para:
        // - confirmar que existe
        // - preservar CreatedAt (histórico)
        var current = await _repo.GetByIdAsync(id, ct);
        if (current is null) return false;

        // Como o GetByIdAsync pode retornar entidade sem tracking,
        // criamos uma nova entidade com os valores do DTO e mantemos CreatedAt.
        var entity = _mapper.Map<Transaction>(dto);
        entity.Id = id;
        entity.CreatedAt = current.CreatedAt;

        return await _repo.UpdateAsync(entity, ct);
    }

    /// <summary>
    /// Remove uma transação pelo identificador.
    /// </summary>
    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => id <= 0 ? Task.FromResult(false) : _repo.DeleteAsync(id, ct);

    /// <summary>
    /// Valida regras mínimas para criação de Transaction.
    /// </summary>
    private static void Validate(TransactionCreateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new ArgumentException("Description é obrigatório.");

        // NOTE: aqui você está tratando Amount como string (provavelmente vem do front assim).
        // O ideal é validar formato numérico também (ex: decimal.TryParse) — dá ponto extra em seleção.
        if (string.IsNullOrWhiteSpace(dto.Amount))
            throw new ArgumentException("Amount é obrigatório.");

        if (dto.CategoryId <= 0)
            throw new ArgumentException("CategoryId inválido.");

        if (dto.PessoaId <= 0)
            throw new ArgumentException("PessoaId inválido.");
    }

    /// <summary>
    /// Valida regras mínimas para atualização de Transaction.
    /// </summary>
    private static void Validate(TransactionUpdateDto dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Description))
            throw new ArgumentException("Description é obrigatório.");

        // NOTE: Amount como string — considere validar formato/intervalo.
        if (string.IsNullOrWhiteSpace(dto.Amount))
            throw new ArgumentException("Amount é obrigatório.");

        if (dto.CategoryId <= 0)
            throw new ArgumentException("CategoryId inválido.");

        if (dto.PessoaId <= 0)
            throw new ArgumentException("PessoaId inválido.");
    }

    /// <summary>
    /// Valida existência das chaves estrangeiras (Category e Pessoa).
    /// </summary>
    /// <remarks>
    /// Isso evita que o banco retorne uma exceção genérica de FK,
    /// permitindo mensagens de erro mais amigáveis e previsíveis.
    /// </remarks>
    private async Task ValidateForeignKeys(int categoryId, int pessoaId, CancellationToken ct)
    {
        if (!await _repo.CategoryExistsAsync(categoryId, ct))
            throw new ArgumentException("CategoryId não existe.");

        if (!await _repo.PessoaExistsAsync(pessoaId, ct))
            throw new ArgumentException("PessoaId não existe.");
    }
}
