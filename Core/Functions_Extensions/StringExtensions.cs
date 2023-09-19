using System.Globalization;
using System.Text.RegularExpressions;

namespace Core.Functions_Extensions;

public static class StringExtensions
{
    public static string CamelToSnakeCase(this string str) => Regex.Replace(str, @"(\p{Ll})(\p{Lu})", "$1_$2").ToLowerInvariant();
    public static string SnakeToCamelCase(this string str)
    {
        string[] words = str.Split('_');
        string result = "";

        for (int i = 0; i < words.Length; i++)
        {
            string word = words[i];

            if (i == 0)
                result += word;

            else
            {
                string capitalizedWord = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word);
                result += capitalizedWord;
            }
        }

        if (result.Length > 0)
            return result;
        return str;
    }
    public static string SnakeToTitleCase(this string str)
    {
        string[] words = str.Split('_');
        string result = "";

        foreach (string word in words)
        {
            string capitalizedWord = CultureInfo.CurrentCulture.TextInfo.ToTitleCase(word);
            result += capitalizedWord;
        }

        if (result.Length > 0)
            return result;

        return CultureInfo.CurrentCulture.TextInfo.ToTitleCase(str);
    }
    public static bool EqualsIgnoreCase(this string str1, string str2) => string.Equals(str1, str2, StringComparison.OrdinalIgnoreCase);
}
