using Microsoft.Extensions.Localization;
using System.Reflection;
namespace UI.Resources;


public class LocService : ILocService
{
    private readonly IStringLocalizer _localizer;

    public LocService(IStringLocalizerFactory factory)
    {
        var type = typeof(SharedResource);
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
        _localizer = factory.Create("SharedResource", assemblyName.Name);
    }

    public string GetLocalizedString(string key, params string[] args) => _localizer[key, args]?.Value ?? key;
}