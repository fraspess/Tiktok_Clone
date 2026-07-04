using Domain.Constants;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.AspNetCore.Mvc.ModelBinding;

namespace Api.Filters;

public class NullActionFilter : IActionFilter
{
    public void OnActionExecuting(ActionExecutingContext context)
    {
        var bodyParameters = context.ActionDescriptor.Parameters
            .Where(p => p.BindingInfo?.BindingSource == BindingSource.Body);

        foreach (var parameter in bodyParameters)
            if (!context.ActionArguments.TryGetValue(parameter.Name, out var value) || value is null)
            {
                context.Result = new BadRequestObjectResult(new
                {
                    isSuccess = false,
                    errors = new[] { "Тіло запиту не має бути порожнім!" }
                });
                return;
            }
    }

    public void OnActionExecuted(ActionExecutedContext context)
    {
    }
}