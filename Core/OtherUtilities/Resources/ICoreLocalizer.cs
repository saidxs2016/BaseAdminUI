namespace Core.OtherUtilities.Resources;


public interface ICoreLocalizer
{
    string GetLocalizedString(string key, params string[] args); // Ortak Dosya
}