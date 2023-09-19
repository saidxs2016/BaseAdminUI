using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Serilog.Core;
using Serilog.Core.Enrichers;
using System.Text;
using System.Text.Json.Nodes;

namespace Core.Functions_Extensions;
public class RequestEnrichersFunction
{
    public static async Task<List<ILogEventEnricher>> EventEnrichers(HttpContext httpContext)
    {
        if (httpContext == null || httpContext.Request == null || httpContext.User == null || httpContext.User.Claims == null || httpContext.Connection == null)
            return null;

        var authKey = httpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.AuthID)?.Value;
        List<ILogEventEnricher> enrichers = new()
        {
            new PropertyEnricher("UserLog", true),
            new PropertyEnricher("RemoteIp", httpContext.Connection.RemoteIpAddress.ToString()),
            new PropertyEnricher("Path", Convert.ToString(httpContext.Request.Path)),
            new PropertyEnricher("AdminUid", Convert.ToString(authKey?.Split(".")[1])),
            new PropertyEnricher("FullName", Convert.ToString(httpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.FullName)?.Value)),
            new PropertyEnricher("RoleName", Convert.ToString(httpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.RoleName)?.Value)),
            new PropertyEnricher("ByUser", Convert.ToString(httpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.ByUser)?.Value)),
            new PropertyEnricher("ByConnectionKey", Convert.ToString(httpContext.User.Claims.FirstOrDefault(w => w.Type == ClaimHelper.ByConnectionKey)?.Value))
        };

        //  ============================ Handler Request ============================
        try
        {
            var queryArr = httpContext.Request.Query.Select(item => new DicHelper { Key = item.Key, Value = item.Value }).ToArray();
            var routeValues = httpContext.Request.RouteValues.Select(item => new { item.Key, item.Value }).ToArray();
            var headers = httpContext.Request.Headers.Select(item => new DicHelper { Key = item.Key, Value = item.Value }).ToArray();


            JsonObject requestInfoAsObj = null;
            List<DicHelper> formDicObj = new();
            if (httpContext.Request.ContentLength > 0)
            {
                httpContext.Request.EnableBuffering();
                var buffer = new byte[Convert.ToInt32(httpContext.Request.ContentLength)];
                httpContext.Request.Body.Position = 0;  //rewinding the stream to 0
                await httpContext.Request.Body.ReadAsync(buffer);
                try
                {
                    requestInfoAsObj = JsonNode.Parse(Encoding.UTF8.GetString(buffer)).AsObject();
                }
                catch (Exception) { requestInfoAsObj = null; }

                httpContext.Request.Body.Position = 0;  //rewinding the stream to 0
            }
            if (requestInfoAsObj is null)
            {
                try
                {
                    var formCollection = await httpContext.Request.ReadFormAsync();
                    if (formCollection != null)
                        formDicObj = formCollection.Select(item => new DicHelper { Key = item.Key, Value = item.Value }).ToList();
                }
                catch (Exception) { }

            }

            var requestInfo = new
            {
                Url = httpContext.Request.GetDisplayUrl(),
                Headers = headers,
                Query = queryArr,
                RouteValues = routeValues,
                Body = requestInfoAsObj,
                Form = formDicObj.ToArray()
            };

            enrichers.Add(new PropertyEnricher("RequestInfo", requestInfo.SerializeWithCamelCase()));
        }
        catch (Exception) { }

        return enrichers;
    }
    public record DicHelper
    {
        public string Key { get; init; }
        public string Value { get; init; }
    }
}