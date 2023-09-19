using Microsoft.Extensions.Localization;
using System.Reflection;

namespace Application.Resources;


public class ApplicationLocalizer : IApplicationLocalizer
{
    private readonly IStringLocalizer _localizer;

    public ApplicationLocalizer(IStringLocalizerFactory factory)
    {
        var type = typeof(SharedResource);
        var assemblyName = new AssemblyName(type.GetTypeInfo().Assembly.FullName);
        _localizer = factory.Create("SharedResource", assemblyName.Name);
    }
 
  
    public string GetLocalizedString(string key, params string[] args) => _localizer[key, args]?.Value ?? key;
}