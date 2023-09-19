using System.Globalization;

namespace UI.Extensions.HttpClient;

public class ApiGatewayHttpClient : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    private readonly ILogger<ApiGatewayHttpClient> _logger;
    public ApiGatewayHttpClient(IHttpContextAccessor httpContextAccessor, ILogger<ApiGatewayHttpClient> logger)
    {
        _httpContextAccessor = httpContextAccessor;
        _logger = logger;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token_cookie = _httpContextAccessor.HttpContext?.Request.Cookies["Access-Token"] ?? null;
        if (!string.IsNullOrEmpty(token_cookie))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token_cookie);

        request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);
        var head = _httpContextAccessor.HttpContext?.Request.Headers["RequestVerificationToken"];
        if (!string.IsNullOrEmpty(head))
        {
            var verify_token = !string.IsNullOrEmpty(head) ? head.ToString() : "";
            request.Headers.Add("RequestVerificationToken", verify_token);
        }



        return base.SendAsync(request, cancellationToken);
    }
}
