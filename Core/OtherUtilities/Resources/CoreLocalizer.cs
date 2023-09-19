using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Core.OtherUtilities.Resources;


public class CoreLocalizer : ICoreLocalizer
{
    private readonly IStringLocalizer _localizer;

    public CoreLocalizer(IStringLocalizerFactory factory)
    {
        var type = typeof(SharedResource);
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
        _localizer = factory.Create("SharedResource", assemblyName.Name);
    }

    public string GetLocalizedString(string key, params string[] args) => _localizer[key, args]?.Value ?? key;
}