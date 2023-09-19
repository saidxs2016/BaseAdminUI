using FluentValidation;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Text.Json;
namespace UI.Filters;

public class ValidationFilterAttribute : TypeFilterAttribute
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="type"> Kullanılacak Validator </param>
    /// <param name="returnType"> return Edilecek Obj tipi = { 0 ise json, 1 ise redirect result olacak} </param>
    public ValidationFilterAttribute(Type type, int returnType = 0) : base(typeof(ValidationFilter))
    {
        Arguments = new object[] { type, returnType };
    }


    private class ValidationFilter : IAsyncActionFilter
    {
        private Type _validatorType;
        private int _returnType;
        public ValidationFilter(Type validatorType, int returnType = 0)
        {
            if (!typeof(IValidator).IsAssignableFrom(validatorType))
                throw new Exception("Doğru verileri girmelisiniz.");

            _validatorType = validatorType;
            _returnType = returnType;
        }          
        public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            var validator = (IValidator)Activator.CreateInstance(_validatorType);
            var entityType = _validatorType.BaseType.GetGenericArguments()[0];
            var entities = context.ActionArguments.Values.Where(t => t.GetType() == entityType);

            List<string> errors = new();
            foreach (var entity in entities)
            {
                var tmp_err = Validate(validator, entity);
                if (!string.IsNullOrEmpty(tmp_err))
                    errors.Add(tmp_err);
            }
            if (errors.Count > 0 && _returnType > 0)
            {
                var data = new { Message = string.Join(", ", errors) };
                string url = $"/ErrorPage/Error?details={JsonSerializer.Serialize(data)}";
                context.Result = new RedirectResult(url);
                return;
            }
            if (errors.Count > 0 && _returnType == 0)
            {
                context.Result = new OkObjectResult(string.Join(", ", errors));
                return;
            }

            await next();


        }

        private static string Validate(IValidator validator, object entity)
        {
            string errors_str = null;
            try
            {
                var context = new ValidationContext<object>(entity);
                var result = validator.Validate(context);
                if (!result.IsValid)
                {
                    var errors = result.Errors.ConvertAll(e => $"\"Prop: {e.PropertyName}\" ==> {e.ErrorMessage}");
                    errors_str = string.Join(", ", errors);
                }
            }
            catch (Exception ex)
            {
                errors_str = ex.Message;
            }
            return errors_str;
        }
    }
}