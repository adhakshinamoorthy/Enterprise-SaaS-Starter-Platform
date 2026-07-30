using System.Diagnostics;
using MediatR;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
namespace EnterpriseSaaS.Common;

public interface ISecretProvider { string? Get(string name); }
public sealed class EnvironmentSecretProvider : ISecretProvider { public string? Get(string name) => Environment.GetEnvironmentVariable(name); }
public sealed class JsonSecretProvider(Microsoft.Extensions.Configuration.IConfiguration configuration) : ISecretProvider { public string? Get(string name) => configuration[$"Secrets:{name}"]; }
public interface ICorrelationContext { string CorrelationId { get; } }
public sealed class CorrelationContext(IHttpContextAccessor accessor) : ICorrelationContext { public string CorrelationId => accessor.HttpContext?.TraceIdentifier ?? Activity.Current?.Id ?? Guid.NewGuid().ToString("N"); }
public sealed class LoggingBehavior<TRequest, TResponse>(ILogger<LoggingBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{ public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct) { logger.LogInformation("Handling {Request}", typeof(TRequest).Name); var response = await next(); logger.LogInformation("Handled {Request}", typeof(TRequest).Name); return response; } }
public sealed class PerformanceBehavior<TRequest, TResponse>(ILogger<PerformanceBehavior<TRequest, TResponse>> logger) : IPipelineBehavior<TRequest, TResponse> where TRequest : notnull
{ public async Task<TResponse> Handle(TRequest request, RequestHandlerDelegate<TResponse> next, CancellationToken ct) { var sw = Stopwatch.StartNew(); var response = await next(); if (sw.ElapsedMilliseconds > 500) logger.LogWarning("Slow request {Request}: {Elapsed}ms", typeof(TRequest).Name, sw.ElapsedMilliseconds); return response; } }
