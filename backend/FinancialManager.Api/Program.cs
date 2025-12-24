using FinancialManager.Api.Middlewares;
using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Mappings;
using FinancialManager.Application.Services;
using FinancialManager.Infrastructure.Data;
using FinancialManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

/*
 * Projeto: FinancialManager
 * Entry point da API (ASP.NET Core).
 *
 * Responsabilidades:
 * - Configurar DI (Services/Repositories)
 * - Configurar EF Core (DbContext)
 * - Configurar Swagger
 * - Configurar CORS
 * - Registrar Middlewares
 */

// Controllers (API REST)
builder.Services.AddControllers();

// Swagger / OpenAPI (útil para testar endpoints durante desenvolvimento e avaliação)
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Banco de dados (SQLite)
// Observação: ConnectionString "Default" fica em appsettings.json
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

/*
 * Dependency Injection
 * - Services (camada Application): regras + validações + orquestração
 * - Repositories (camada Infrastructure): persistência/consultas via EF Core
 */
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddScoped<ICategoryService, CategoryService>();
builder.Services.AddScoped<ITransactionService, TransactionService>();
builder.Services.AddScoped<IDashboardService, DashboardService>();

// AutoMapper
// Ponto importante: basta registrar UMA vez com o assembly onde estão os Profiles.
// Como os Profiles estão no mesmo assembly, um AddAutoMapper já encontra todos.
builder.Services.AddAutoMapper(typeof(PessoaProfile).Assembly);
// (Opcional) remover estes abaixo para evitar repetição:
// builder.Services.AddAutoMapper(typeof(CategoryProfile).Assembly);
// builder.Services.AddAutoMapper(typeof(TransactionProfile).Assembly);

builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();
builder.Services.AddScoped<ICategoryRepository, CategoryRepository>();
builder.Services.AddScoped<ITransactionRepository, TransactionRepository>();
builder.Services.AddScoped<IDashboardRepository, DashboardRepository>();

// CORS: libera o front (Vite) em localhost durante desenvolvimento
builder.Services.AddCors(options =>
{
    options.AddPolicy("frontend", policy =>
        policy
            .WithOrigins("http://localhost:5173")
            .AllowAnyHeader()
            .AllowAnyMethod()
    );
});

var app = builder.Build();

/*
 * Middlewares
 * - ExceptionHandlingMiddleware: traduz exceções de regra (ex: ArgumentException)
 *   em respostas HTTP adequadas (ex: 400), padronizando erros para o cliente.
 */
app.UseMiddleware<ExceptionHandlingMiddleware>();

// Swagger somente em Development (boa prática)
if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

// Pipeline HTTP
app.UseCors("frontend");
app.UseHttpsRedirection();

app.MapControllers();
app.Run();
