using FluentValidation;

namespace Core.OtherUtilities.CrossCuttingConcerns.Validation;

public static class ValidationTool
{
    public static void Validate(IValidator validator, object entity)
    {
        var context = new ValidationContext<object>(entity);
        var result = validator.Validate(context);
        if (!result.IsValid)
        {
            var errors = result.Errors.Select(e => e.PropertyName + ": " + e.ErrorMessage).ToArray();
            var errors_str = string.Join(", ", errors);
            throw new ValidationException(errors_str);
        }
    }
}
