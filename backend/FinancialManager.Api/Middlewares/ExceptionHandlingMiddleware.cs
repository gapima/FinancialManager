using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManager.Api.Middlewares;

/*
 * Projeto: FinancialManager
 * Camada: Api (Middleware)
 * Responsabilidade:
 * - Interceptar exceções não tratadas no pipeline HTTP.
 * - Traduzir exceções em respostas HTTP padronizadas (Problem Details - RFC 7807).
 *
 * Benefícios:
 * - Evita propagação de stacktrace para o cliente.
 * - Padroniza erros para consumo no front-end.
 * - Facilita debug via traceId.
 *
 * Convenções adotadas:
 * - ArgumentException      -> 400 BadRequest
 * - KeyNotFoundException   -> 404 NotFound
 * - Qualquer outra exceção -> 500 InternalServerError
 */
public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    /// <summary>
    /// Constrói o middleware de tratamento global de exceções.
    /// </summary>
    /// <param name="next">Próximo delegate no pipeline HTTP.</param>
    /// <param name="logger">Logger para registrar exceções.</param>
    public ExceptionHandlingMiddleware(
        RequestDelegate next,
        ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    /// <summary>
    /// Executa o middleware.
    /// </summary>
    /// <remarks>
    /// Envolve a execução do pipeline em um try/catch global,
    /// garantindo que qualquer exceção não tratada seja convertida
    /// em uma resposta HTTP consistente.
    /// </remarks>
    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            // Loga a exceção completa (stacktrace fica no servidor)
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            // Mapeia exceção para status HTTP e título amigável
            var (statusCode, title) = ex switch
            {
                ArgumentException =>
                    ((int)HttpStatusCode.BadRequest, "Requisição inválida"),

                KeyNotFoundException =>
                    ((int)HttpStatusCode.NotFound, "Recurso não encontrado"),

                _ =>
                    ((int)HttpStatusCode.InternalServerError, "Erro interno do servidor")
            };

            // Padrão RFC 7807 (Problem Details)
            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = ex.Message, // mensagem de negócio, quando aplicável
                Instance = context.Request.Path
            };

            // TraceId ajuda muito em debug e correlação de logs
            problem.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var json = JsonSerializer.Serialize(
                problem,
                new JsonSerializerOptions
                {
                    PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                });

            await context.Response.WriteAsync(json);
        }
    }
}
