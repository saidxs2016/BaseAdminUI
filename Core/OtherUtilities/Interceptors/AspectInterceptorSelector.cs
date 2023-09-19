using Castle.DynamicProxy;
using System.Reflection;

namespace Core.OtherUtilities.Interceptors;

public class AspectInterceptorSelector : IInterceptorSelector
{
    public IInterceptor[] SelectInterceptors(Type type, MethodInfo method, IInterceptor[] interceptors)
    {
        var classAttributes = type.GetCustomAttributes<MethodInterceptionBaseAttribute>
           (true).ToList();
        var methodAttributes = type.GetMethod(method.Name)
            .GetCustomAttributes<MethodInterceptionBaseAttribute>(true);
        classAttributes.AddRange(methodAttributes);


        /////////////////////aşağıdaki gibi bir ekleme yaparsak, mevcuttaki tüm metodlara bu aspect ekler. 
        ////çalışan yapı////classAttributes.Add(new ExceptionLogAspect(typeof(FileLogger))); //loglama için

        return classAttributes.OrderBy(x => x.Priority).ToArray();
    }
}
