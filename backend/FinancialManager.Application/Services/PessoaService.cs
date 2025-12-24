using AutoMapper;
using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Contracts.Pessoa;
using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Services;

/*
 * Projeto: FinancialManager
 * Camada: Application
 * Responsabilidade:
 * - Implementar os casos de uso relacionados à entidade Pessoa (CRUD)
 *
 * Observações de design:
 * - A camada Application orquestra o fluxo: valida entrada, chama repositório e mapeia DTOs.
 * - As regras básicas de consistência (Nome obrigatório, Idade não-negativa) são validadas aqui
 *   para impedir persistência de dados inválidos.
 * - O serviço depende apenas de abstrações (IPessoaRepository) e de um mapper (AutoMapper),
 *   evitando acoplamento com Infra/EF diretamente.
 */
public sealed class PessoaService : IPessoaService
{
    private readonly IPessoaRepository _repo;
    private readonly IMapper _mapper;

    /// <summary>
    /// Constrói o serviço de Pessoa.
    /// </summary>
    /// <param name="repo">Abstração de persistência de Pessoa (implementada na Infrastructure).</param>
    /// <param name="mapper">Responsável por mapear DTOs &lt;-&gt; Entidades de domínio.</param>
    public PessoaService(IPessoaRepository repo, IMapper mapper)
    {
        _repo = repo;
        _mapper = mapper;
    }

    /// <summary>
    /// Retorna todas as pessoas cadastradas.
    /// </summary>
    /// <param name="ct">Token de cancelamento para evitar trabalhos desnecessários.</param>
    /// <returns>Lista de pessoas no formato de resposta (DTO).</returns>
    public async Task<List<PessoaResponseDto>> GetAllAsync(CancellationToken ct = default)
    {
        // A camada de Application não expõe entidades do domínio diretamente.
        // Aqui, retornamos DTOs para manter o contrato desacoplado do modelo interno.
        var pessoas = await _repo.GetAllAsync(ct);
        return _mapper.Map<List<PessoaResponseDto>>(pessoas);
    }

    /// <summary>
    /// Retorna uma pessoa pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da pessoa (deve ser &gt; 0).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// DTO da pessoa, ou null quando o id é inválido ou o registro não existe.
    /// </returns>
    public async Task<PessoaResponseDto?> GetByIdAsync(int id, CancellationToken ct = default)
    {
        // Regra: ids não positivos são tratados como inválidos.
        // Retornar null simplifica o fluxo (Controller decide 404/400).
        if (id <= 0) return null;

        var pessoa = await _repo.GetByIdAsync(id, ct);
        return pessoa is null ? null : _mapper.Map<PessoaResponseDto>(pessoa);
    }

    /// <summary>
    /// Cria uma nova pessoa.
    /// </summary>
    /// <param name="dto">Dados necessários para criação.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>Pessoa criada no formato de resposta (DTO).</returns>
    /// <exception cref="ArgumentException">Quando os dados de entrada são inválidos.</exception>
    public async Task<PessoaResponseDto> CreateAsync(PessoaCreateDto dto, CancellationToken ct = default)
    {
        // Validação antes de persistir: evita dados inválidos no banco e falhas posteriores.
        Validate(dto.Nome, dto.Idade);

        var pessoa = _mapper.Map<Pessoa>(dto);

        var created = await _repo.AddAsync(pessoa, ct);

        return _mapper.Map<PessoaResponseDto>(created);
    }

    /// <summary>
    /// Atualiza uma pessoa existente.
    /// </summary>
    /// <param name="id">Identificador da pessoa (deve ser &gt; 0).</param>
    /// <param name="dto">Dados para atualização.</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>
    /// True quando atualizou com sucesso; False quando id é inválido ou não encontrou o registro.
    /// </returns>
    /// <exception cref="ArgumentException">Quando os dados de entrada são inválidos.</exception>
    public async Task<bool> UpdateAsync(int id, PessoaUpdateDto dto, CancellationToken ct = default)
    {
        if (id <= 0) return false;

        Validate(dto.Nome, dto.Idade);

        // Busca antes de atualizar para garantir que o registro existe
        // e evitar update "cego" (e.g., retornar true sem alterar nada).
        var pessoa = await _repo.GetByIdAsync(id, ct);
        if (pessoa is null) return false;

        // Mapeia apenas campos atualizáveis do DTO para a entidade existente.
        // Isso mantém a entidade como fonte de verdade e evita recriar objeto.
        _mapper.Map(dto, pessoa);

        // Garantia: id do path tem precedência sobre qualquer valor que venha do DTO.
        pessoa.Id = id;

        return await _repo.UpdateAsync(pessoa, ct);
    }

    /// <summary>
    /// Remove uma pessoa pelo identificador.
    /// </summary>
    /// <param name="id">Identificador da pessoa (deve ser &gt; 0).</param>
    /// <param name="ct">Token de cancelamento.</param>
    /// <returns>True se removeu; False se id é inválido ou não removeu.</returns>
    public Task<bool> DeleteAsync(int id, CancellationToken ct = default)
        => id <= 0 ? Task.FromResult(false) : _repo.DeleteAsync(id, ct);

    /// <summary>
    /// Valida regras mínimas de consistência para Pessoa.
    /// </summary>
    /// <param name="nome">Nome da pessoa (obrigatório).</param>
    /// <param name="idade">Idade (não pode ser negativa).</param>
    /// <exception cref="ArgumentException">Quando as regras de consistência falham.</exception>
    private static void Validate(string nome, int idade)
    {
        if (string.IsNullOrWhiteSpace(nome))
            throw new ArgumentException("Nome é obrigatório.");

        if (idade < 0)
            throw new ArgumentException("Idade inválida.");
    }
}
