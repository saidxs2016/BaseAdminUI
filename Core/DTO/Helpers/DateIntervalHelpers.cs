namespace Core.DTO.Helpers;

public class DateIntervalHelpers
{
    public const string Minute = "mi";
    public const string Hour = "h";
    public const string Day = "d";
    public const string Year = "y";

    public static Dictionary<string, string> Intervals = new() { { "mi", "Dakika" }, { "h", "Saat" }, { "d", "Gün" }, { "y", "Yıl" } };
    public static List<string> IntervalsKeys = Intervals.Keys.ToList();

    public static int GetInternalAsMinutes(string key)
    {
        if (string.IsNullOrEmpty(key))
            return 0;

        var keyParams = key.Split(" ");
        if (keyParams.Length != 2 || !IntervalsKeys.Contains(keyParams[1]))
            return 0;

        if (keyParams[1] == Minute && int.TryParse(keyParams[0], out int minute))
            return minute;

        if (keyParams[1] == Hour && int.TryParse(keyParams[0], out int hour))
            return hour * 60;

        if (keyParams[1] == Day && int.TryParse(keyParams[0], out int day))
            return day * 24 * 60;

        if (keyParams[1] == Year && int.TryParse(keyParams[0], out int year))
            return year * 365 * 24 * 60;

        return 0;
    }
}
