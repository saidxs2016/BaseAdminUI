using System.Globalization;

namespace UI.Extensions.HttpClient;

public class AuthSystemUserHttpClient : DelegatingHandler
{
    private readonly IHttpContextAccessor _httpContextAccessor;
    public AuthSystemUserHttpClient(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    protected override Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
    {
        var token_cookie = _httpContextAccessor.HttpContext?.Request.Cookies["Access-Token"] ?? null;
        if (!string.IsNullOrEmpty(token_cookie))
            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("bearer", token_cookie);

        request.Headers.Add("Accept-Language", CultureInfo.CurrentCulture.Name);



        return base.SendAsync(request, cancellationToken);
    }
}
