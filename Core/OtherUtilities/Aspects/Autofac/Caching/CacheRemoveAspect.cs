using Castle.DynamicProxy;
using Core.OtherUtilities.CrossCuttingConcerns.Caching;
using Core.OtherUtilities.Interceptors;
using Core.OtherUtilities.IoC;
using Microsoft.Extensions.DependencyInjection;

namespace Core.OtherUtilities.Aspects.Autofac.Caching;

/* KULLANIMI
 * [CacheRemoveAspect("IProductService.Get")] // örnek ekleme yaptığında güncellenmesi gereken servisleri temizlemyecek ( önemli : bu isimde olanları temizleyecek )
 */
public class CacheRemoveAspect : MethodInterception
{
    private readonly string _pattern;
    private readonly ICacheManager _cacheManager;

    public CacheRemoveAspect(string pattern)
    {
        _pattern = pattern;
        _cacheManager = ServiceTool.ServiceProvider.GetService<ICacheManager>();
    }

    protected override void OnSuccess(IInvocation invocation)
    {
        _cacheManager.RemoveByPattern(_pattern);
    }
}
