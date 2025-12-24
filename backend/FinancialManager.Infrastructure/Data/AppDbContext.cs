using FinancialManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;

namespace FinancialManager.Infrastructure.Data;

/*
 * Projeto: FinancialManager
 * Camada: Infrastructure (Data)
 * Responsabilidade:
 * - Definir o DbContext do EF Core, registrando DbSets e mapeamentos (Fluent API).
 *
 * Observações de design:
 * - A camada Infrastructure concentra detalhes de persistência (tabelas, constraints, relacionamentos).
 * - Regras como "o que acontece ao deletar" (Cascade/Restrict) são decisões de integridade referencial
 *   e ficam explicitadas no mapping para evitar comportamento implícito.
 */
public class AppDbContext : DbContext
{
    /// <summary>
    /// Constrói o contexto EF Core usando opções fornecidas pela DI.
    /// </summary>
    /// <param name="options">Opções do DbContext (provider, connection string, etc.).</param>
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    /// <summary>
    /// Conjunto persistido de Pessoas.
    /// </summary>
    public DbSet<Pessoa> Pessoas => Set<Pessoa>();

    /// <summary>
    /// Conjunto persistido de Categorias.
    /// </summary>
    public DbSet<Category> Categories => Set<Category>();

    /// <summary>
    /// Conjunto persistido de Transações.
    /// </summary>
    public DbSet<Transaction> Transactions => Set<Transaction>();

    /// <summary>
    /// Configura o mapeamento das entidades (Fluent API).
    /// </summary>
    /// <remarks>
    /// Por padrão, as configurações aqui definem:
    /// - Nome das tabelas
    /// - Regras de obrigatoriedade (IsRequired)
    /// - Tamanhos (HasMaxLength)
    /// - Precisão numérica (HasPrecision)
    /// - Relacionamentos e comportamento de deleção (OnDelete)
    /// </remarks>
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        // =========================
        // Pessoa
        // =========================
        modelBuilder.Entity<Pessoa>(e =>
        {
            e.ToTable("Pessoas");

            // Nome obrigatório e limitado para evitar strings enormes no DB.
            e.Property(x => x.Nome)
                .IsRequired()
                .HasMaxLength(200);

            // Idade obrigatória (validação de regra também acontece na Application).
            e.Property(x => x.Idade)
                .IsRequired();
        });

        // =========================
        // Category
        // =========================
        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Categories");

            e.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(200);

            // Purpose é obrigatório (pode ser enum/string conforme sua entidade).
            e.Property(x => x.Purpose)
                .IsRequired();
        });

        // =========================
        // Transaction
        // =========================
        modelBuilder.Entity<Transaction>(e =>
        {
            e.ToTable("Transactions");

            e.Property(x => x.Description)
                .IsRequired()
                .HasMaxLength(250);

            // Precisão definida para valores monetários.
            e.Property(x => x.Amount)
                .IsRequired()
                .HasPrecision(18, 2);

            e.Property(x => x.Type)
                .IsRequired();

            e.Property(x => x.CreatedAt)
                .IsRequired();

            /*
             * Relacionamentos e integridade referencial
             *
             * Pessoa -> Transaction
             * - Regra: ao excluir uma Pessoa, suas Transações devem ser excluídas junto.
             * - Motivo: Transação depende da existência de Pessoa (não faz sentido órfã).
             */
            e.HasOne<Pessoa>()
                .WithMany()
                .HasForeignKey(x => x.PessoaId)
                .OnDelete(DeleteBehavior.Cascade);

            /*
             * Category -> Transaction
             * - Regra: uma Categoria não deve ser removida se existir Transação vinculada.
             * - Motivo: evita perda de referência histórica e inconsistência em relatórios.
             * - DeleteBehavior.Restrict força o banco a impedir deleção enquanto houver vínculo.
             */
            e.HasOne<Category>()
                .WithMany()
                .HasForeignKey(x => x.CategoryId)
                .OnDelete(DeleteBehavior.Restrict);
        });

        base.OnModelCreating(modelBuilder);
    }
}
