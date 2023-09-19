using System.Text.Json;

namespace Core.Functions_Extensions;

public static class JsonSerializerExtensions
{
    /// <summary>
    /// Serileştirilince baş harfleri küçültür
    /// </summary> 
    public static string SerializeWithCamelCase<T>(this T data)
    {
        return JsonSerializer.Serialize(data, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
            //Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
        });
    }
    /// <summary>
    /// Deserialize ederken baş harfleri büyütür
    /// </summary> 
    public static T DeserializeFromCamelCase<T>(this string json)
    {
        return JsonSerializer.Deserialize<T>(json, new JsonSerializerOptions
        {
            PropertyNamingPolicy = JsonNamingPolicy.CamelCase
        });
    }

    



}
