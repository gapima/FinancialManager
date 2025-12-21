using FinancialManager.Application.Abstractions.Repository;
using FinancialManager.Application.Abstractions.Service;
using FinancialManager.Application.Mappings;
using FinancialManager.Application.Services;
using FinancialManager.Infrastructure.Data;
using FinancialManager.Infrastructure.Repositories;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllers();

// Swagger
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

// Db
builder.Services.AddDbContext<AppDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("Default")));

// Services
builder.Services.AddScoped<IPessoaService, PessoaService>();
builder.Services.AddAutoMapper(typeof(PessoaProfile).Assembly);

// Repository
builder.Services.AddScoped<IPessoaRepository, PessoaRepository>();

var app = builder.Build();

if (app.Environment.IsDevelopment())
{
    app.UseSwagger();
    app.UseSwaggerUI();
}

app.UseHttpsRedirection();
app.MapControllers();
app.Run();
