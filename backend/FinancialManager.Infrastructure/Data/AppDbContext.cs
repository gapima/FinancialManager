using FinancialManager.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using System;

namespace FinancialManager.Infrastructure.Data;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<Pessoa> Pessoas => Set<Pessoa>();
    public DbSet<Category> Categories => Set<Category>();
    public DbSet<Transaction> Transactions => Set<Transaction>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Pessoa>(e =>
        {
            e.ToTable("Pessoas");
            e.Property(x => x.Nome).IsRequired().HasMaxLength(200);
            e.Property(x => x.Idade).IsRequired();
        });

        modelBuilder.Entity<Category>(e =>
        {
            e.ToTable("Categories");
            e.Property(x => x.Description).IsRequired().HasMaxLength(200);
            e.Property(x => x.Purpose).IsRequired();
        });

        modelBuilder.Entity<Transaction>(e =>
        {
            e.ToTable("Transactions");
            e.Property(x => x.Description).IsRequired().HasMaxLength(250);
            e.Property(x => x.Amount).IsRequired().HasPrecision(18, 2);
            e.Property(x => x.Type).IsRequired();
            e.Property(x => x.CreatedAt).IsRequired();

            // Deletar pessoa apaga transações
            e.HasOne<Pessoa>()
             .WithMany()
             .HasForeignKey(x => x.PessoaId)
             .OnDelete(DeleteBehavior.Cascade);

            // Categoria não deve sumir se tem transação vinculada
            e.HasOne<Category>()
             .WithMany()
             .HasForeignKey(x => x.CategoryId)
             .OnDelete(DeleteBehavior.Restrict);
        });
    }
}
