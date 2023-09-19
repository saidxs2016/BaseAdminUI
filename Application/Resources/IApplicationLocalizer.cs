namespace Application.Resources;


public interface IApplicationLocalizer
{
    string GetLocalizedString(string key, params string[] args);
}