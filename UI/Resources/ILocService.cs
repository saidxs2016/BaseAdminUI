namespace UI.Resources;


public interface ILocService
{
    string GetLocalizedString(string key, params string[] args);
}