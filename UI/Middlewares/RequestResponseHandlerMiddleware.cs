using Core.Functions_Extensions;
using Serilog.Context;
using Serilog.Core;
using Serilog.Core.Enrichers;
using System.Diagnostics;
using System.Text.Json.Nodes;

namespace UI.Middlewares;

public class RequestResponseHandlerMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<RequestResponseHandlerMiddleware> _logger;
    public RequestResponseHandlerMiddleware(RequestDelegate next, ILogger<RequestResponseHandlerMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }
    public async Task Invoke(HttpContext httpContext)
    {
        var spw = Stopwatch.StartNew();
        // Store the original response body stream
        var originalResponseBody = httpContext.Response.Body;

        try
        {
            var enrichers = await RequestEnrichersFunction.EventEnrichers(httpContext);


            // Create a new memory stream to store the response body
            using MemoryStream responseBodyStream = new();
            // Set the response body to the new memory stream
            httpContext.Response.Body = responseBodyStream;

            // Call the next middleware in the pipeline
            await _next.Invoke(httpContext);

            // Reset the response body stream position to the beginning
            responseBodyStream.Seek(0, SeekOrigin.Begin);

            // Read the response body content
            var responseBody = await new StreamReader(responseBodyStream).ReadToEndAsync();

            // Reset the response body stream position to the beginning again (in case other middleware needs to read it)
            responseBodyStream.Seek(0, SeekOrigin.Begin);

            // Copy the response body back to the original response body stream
            await responseBodyStream.CopyToAsync(originalResponseBody);

            // Log or process the response body as needed

            spw.Stop();
            LogResponse(httpContext.Request.Method, httpContext.Request.Path, responseBody, enrichers, spw.ElapsedMilliseconds);
        }
        catch (Exception)
        {
            spw.Stop();
            throw;
        }
        finally
        {
            // Restore the original response body stream
            httpContext.Response.Body = originalResponseBody;
        }
    }

    private void LogResponse(string request_method, string path, string response, List<ILogEventEnricher> enrichers, long interval = 0)
    {
        _ = Task.Run(() =>
        {
            if (request_method.ToLowerInvariant() != "post" || path.ToLowerInvariant().Contains("/css/") || path.ToLowerInvariant().Contains("/theme/"))
                return;
            enrichers ??= new List<ILogEventEnricher>();           
            var response_obj = JsonNode.Parse(response);
            _ = bool.TryParse(response_obj["responseLogging"]?.ToString(), out bool responseLogging);
            var message = response_obj["message"]?.ToString() ?? "";

            if (response_obj != null && responseLogging)
            {
                enrichers.Add(new PropertyEnricher("StatusCode", StatusCodes.Status200OK));
                enrichers.Add(new PropertyEnricher("LogLevel", "Info"));
                enrichers.Add(new PropertyEnricher("RequestInterval", interval.ToString()));

                using (LogContext.Push(enrichers.ToArray()))
                {
                    _logger.LogWarning(message);
                }
            }

        });


    }
}

public static class RequestResponseMiddleware
{
    public static WebApplication UseRequestResponseMiddleware(this WebApplication app)
    {
        app.UseMiddleware<RequestResponseHandlerMiddleware>();
        return app;
    }
}