using Castle.DynamicProxy;
using Core.OtherUtilities.CrossCuttingConcerns.Validation;
using Core.OtherUtilities.Interceptors;
using FluentValidation;

namespace Core.OtherUtilities.Aspects.Autofac.Validation;

public class ValidationAspect : MethodInterception
{
    /**
     *  KULLANIMI
     * [ValidationAspect(typeof(ProductValidator))]
    */
    private readonly Type _validatorType;
    public ValidationAspect(Type validatorType)
    {
        if (!typeof(IValidator).IsAssignableFrom(validatorType))
        {
            throw new System.Exception("Doğru verileri girlemisiniz");
        }

        _validatorType = validatorType;
    }
    protected override void OnBefore(IInvocation invocation)
    {
        var validator = (IValidator)Activator.CreateInstance(_validatorType);
        var entityType = _validatorType.BaseType.GetGenericArguments()[0];
        var entities = invocation.Arguments.Where(t => t.GetType() == entityType);
        foreach (var entity in entities)
        {
            ValidationTool.Validate(validator, entity);
        }

    }
}
