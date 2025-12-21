using System.Net;
using System.Text.Json;
using Microsoft.AspNetCore.Mvc;

namespace FinancialManager.Api.Middlewares;

public sealed class ExceptionHandlingMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlingMiddleware> _logger;

    public ExceptionHandlingMiddleware(RequestDelegate next, ILogger<ExceptionHandlingMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Unhandled exception: {Message}", ex.Message);

            var (statusCode, title) = ex switch
            {
                ArgumentException => ((int)HttpStatusCode.BadRequest, "Requisição inválida"),
                KeyNotFoundException => ((int)HttpStatusCode.NotFound, "Recurso não encontrado"),
                _ => ((int)HttpStatusCode.InternalServerError, "Erro interno do servidor")
            };

            var problem = new ProblemDetails
            {
                Status = statusCode,
                Title = title,
                Detail = ex is not null ? ex.Message : null,
                Instance = context.Request.Path
            };

            // Ajuda bastante em debug/trace
            problem.Extensions["traceId"] = context.TraceIdentifier;

            context.Response.Clear();
            context.Response.StatusCode = statusCode;
            context.Response.ContentType = "application/problem+json";

            var json = JsonSerializer.Serialize(problem, new JsonSerializerOptions
            {
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase
            });

            await context.Response.WriteAsync(json);
        }
    }
}
