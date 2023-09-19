using Castle.DynamicProxy;
using Core.OtherUtilities.CrossCuttingConcerns.Caching;
using Core.OtherUtilities.Interceptors;
using Core.OtherUtilities.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.OtherUtilities.Aspects.Autofac.Caching;

/* KULLANIMI
 * [CacheAspect(1)] /// bir dakika cache den gelecek sonra normal işlemlere devam edecek
 * */
public class CacheAspect : MethodInterception
{
    private readonly int _duration;
    private readonly ICacheManager _cacheManager;

    public CacheAspect(int duration = 60)
    {
        _duration = duration;
        _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
    }

    public override void Intercept(IInvocation invocation)
    {
        var methodName = string.Format($"{invocation.Method.ReflectedType.FullName}.{invocation.Method.Name}");
        var arguments = invocation.Arguments.ToList();
        var key = $"{methodName}({string.Join(",", arguments.Select(x => x?.ToString() ?? "<Null>"))})";
        if (_cacheManager.IsAdd(key))
        {
            invocation.ReturnValue = _cacheManager.Get(key);
            return;
        }
        invocation.Proceed();

        _cacheManager.Add(key, invocation.ReturnValue, _duration);


    }

   
}
