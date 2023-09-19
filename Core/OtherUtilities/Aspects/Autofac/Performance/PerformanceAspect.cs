using Castle.DynamicProxy;
using Core.OtherUtilities.Interceptors;
using Core.OtherUtilities.IoC;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace Core.OtherUtilities.Aspects.Autofac.Performance;

/**
 * 
 * 
 * 
 * 
 *  KULLANIMI *
 *  
 *  
 *  
 *  
 *  
 *  [PerformanceAspect(5)] /// methodun üzerine yazıyoruz kaç saniye geçerse işlemler yapılsın yani mail gönderilsin 
 **/
public class PerformanceAspect : MethodInterception
{
    private readonly int _interval;
    private readonly Stopwatch _stopwatch;
    private readonly ILogger<PerformanceAspect> _logger;

    public PerformanceAspect(int interval)
    {
        _interval = interval;
        _stopwatch = ServiceTool.ServiceProvider.GetService<Stopwatch>();
        _logger = ServiceTool.ServiceProvider.GetService<ILoggerFactory>().CreateLogger<PerformanceAspect>();
    }
    protected override void OnBefore(IInvocation invocation)
    {
        _stopwatch.Start();
    }
    protected override void OnAfter(IInvocation invocation)
    {
        if (_stopwatch.Elapsed.TotalSeconds > _interval)
        {
            // bu alanda isterseniz mail isterseniz loga ekleyebilirsiniz 
            //Debug.WriteLine($"Performence : {invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}-->{_stopwatch.Elapsed.TotalSeconds}");
            _logger.LogWarning($"Performence : {invocation.Method.DeclaringType.FullName}.{invocation.Method.Name}-->{_stopwatch.Elapsed.TotalSeconds}");
        }
        _stopwatch.Reset();
    }
}
