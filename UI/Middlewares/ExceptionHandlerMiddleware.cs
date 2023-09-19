using Application.DTO.Exceptions;
using Application.DTO.ResultType;
using Core.DTO.Enums;
using Core.Functions_Extensions;
using FluentValidation;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using System.Diagnostics;
using System.Net;

namespace UI.Middlewares;

public class ExceptionHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<ExceptionHandlerMiddleware> _logger;
    public ExceptionHandlerMiddleware(RequestDelegate next, ILogger<ExceptionHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task Invoke(HttpContext httpContext)
    {
        var spw = Stopwatch.StartNew();
        try
        {
            await _next.Invoke(httpContext);
        }
        catch (BadHttpRequestException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            _ = Task.Run(async () =>
            {
                spw.Stop();
                var enrichers = await RequestEnrichersFunction.EventEnrichers(httpContext) ?? new List<ILogEventEnricher>();
                enrichers.Add(new PropertyEnricher("StatusCode", httpContext.Response.StatusCode));
                enrichers.Add(new PropertyEnricher("LogLevel", "Warning"));
                enrichers.Add(new PropertyEnricher("RequestInterval", spw.ElapsedMilliseconds.ToString()));
                using (LogContext.Push(enrichers.ToArray()))
                {
                    _logger.LogError(ex, ex.Message);
                }
            });


            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new Result<object>(IsSuccess: false, Message: ex.Message));
        }
        catch (MVCUnauthorizedAccessException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            _ = Task.Run(async () =>
            {
                spw.Stop();
                var enrichers = await RequestEnrichersFunction.EventEnrichers(httpContext) ?? new List<ILogEventEnricher>();
                enrichers.Add(new PropertyEnricher("StatusCode", httpContext.Response.StatusCode));
                enrichers.Add(new PropertyEnricher("LogLevel", "Warning"));
                enrichers.Add(new PropertyEnricher("RequestInterval", spw.ElapsedMilliseconds.ToString()));
                using (LogContext.Push(enrichers.ToArray()))
                {
                    _logger.LogError(ex, ex.Message);
                }
            });


            httpContext.Response.Redirect($"/errorpage/error?status=401&details={WebUtility.UrlEncode(ex.Message)}");
        }
        catch (JsUnauthorizedAccessException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status401Unauthorized;
            _ = Task.Run(async () =>
            {
                spw.Stop();
                var enrichers = await RequestEnrichersFunction.EventEnrichers(httpContext) ?? new List<ILogEventEnricher>();
                enrichers.Add(new PropertyEnricher("StatusCode", httpContext.Response.StatusCode));
                enrichers.Add(new PropertyEnricher("LogLevel", "Warning"));
                enrichers.Add(new PropertyEnricher("RequestInterval", spw.ElapsedMilliseconds.ToString()));
                using (LogContext.Push(enrichers.ToArray()))
                {
                    _logger.LogError(ex, ex.Message);
                }
            });


            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new Result<object>(IsSuccess: false, Message: ex.Message));
        }

        catch (HttpRequestException ex)
        {
            var error_code = ex.StatusCode;
            if (ex.StatusCode == null)
                error_code = System.Net.HttpStatusCode.ServiceUnavailable;

            httpContext.Response.StatusCode = (int)error_code;
            _ = Task.Run(async () =>
            {
                spw.Stop();
                var enrichers = await RequestEnrichersFunction.EventEnrichers(httpContext) ?? new List<ILogEventEnricher>();
                enrichers.Add(new PropertyEnricher("StatusCode", httpContext.Response.StatusCode));
                enrichers.Add(new PropertyEnricher("LogLevel", "Warning"));
                enrichers.Add(new PropertyEnricher("RequestInterval", spw.ElapsedMilliseconds.ToString()));
                using (LogContext.Push(enrichers.ToArray()))
                {
                    _logger.LogError(ex, ex.Message);
                }
            });



            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new Result<object>(IsSuccess: false, Message: ex.Message));
        }
        catch (ValidationException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            var errors = string.Join(", ", ex.Errors.Select(w => w.ErrorMessage).ToArray());
            _ = Task.Run(async () =>
            {
                spw.Stop();
                var enrichers = await RequestEnrichersFunction.EventEnrichers(httpContext) ?? new List<ILogEventEnricher>();
                enrichers.Add(new PropertyEnricher("StatusCode", httpContext.Response.StatusCode));
                enrichers.Add(new PropertyEnricher("LogLevel", "Warning"));
                enrichers.Add(new PropertyEnricher("RequestInterval", spw.ElapsedMilliseconds.ToString()));
                using (LogContext.Push(enrichers.ToArray()))
                {
                    _logger.LogError(ex, ex.Message);
                }
            });


            //await httpContext.Response.WriteAsync(ex.Message);

            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new Result<object>(IsSuccess: false, Message: errors));
        }

        catch (ModelException ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status400BadRequest;
            _ = Task.Run(async () =>
            {
                spw.Stop();
                var internalLevel = ex.Result.ResultType == ResultTypeEnum.Warning ? "Warning" : "Error";

                var enrichers = await RequestEnrichersFunction.EventEnrichers(httpContext) ?? new List<ILogEventEnricher>();
                enrichers.Add(new PropertyEnricher("StatusCode", httpContext.Response.StatusCode));
                enrichers.Add(new PropertyEnricher("LogLevel", internalLevel));
                enrichers.Add(new PropertyEnricher("RequestInterval", spw.ElapsedMilliseconds.ToString()));
                using (LogContext.Push(enrichers.ToArray()))
                {
                    _logger.LogError(ex, string.IsNullOrEmpty(ex.Message) ? ex.Result.Message : ex.Message);
                }
            });


            //await httpContext.Response.WriteAsync(ex.Message);

            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(ex.Result);
        }

        catch (Exception ex)
        {
            httpContext.Response.StatusCode = StatusCodes.Status500InternalServerError;
            _ = Task.Run(async () =>
            {
                spw.Stop();
                var enrichers = await RequestEnrichersFunction.EventEnrichers(httpContext) ?? new List<ILogEventEnricher>();
                enrichers.Add(new PropertyEnricher("StatusCode", httpContext.Response.StatusCode));
                enrichers.Add(new PropertyEnricher("LogLevel", "Error"));
                enrichers.Add(new PropertyEnricher("RequestInterval", spw.ElapsedMilliseconds.ToString()));
                using (LogContext.Push(enrichers.ToArray()))
                {
                    _logger.LogError(ex, ex.Message);
                }
            });

            //await httpContext.Response.WriteAsync(ex.Message);

            httpContext.Response.ContentType = "application/json";
            await httpContext.Response.WriteAsJsonAsync(new Result<object>(IsSuccess: false, Message: ex.Message));
        }
        finally
        {
            spw.Stop();
        }

    }

}

public static class ExceptionMiddleware
{
    public static WebApplication UseExceptionMiddleware(this WebApplication app)
    {
        app.UseMiddleware<ExceptionHandlerMiddleware>();
        return app;
    }
}
