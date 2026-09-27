using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public class DevelopmentOnlyAttribute : Attribute, IResourceFilter
{
    public void OnResourceExecuting(ResourceExecutingContext context)
    {
        var env = context.HttpContext.RequestServices.GetRequiredService<IWebHostEnvironment>();
        
        if (!env.IsDevelopment())
        {
            context.Result = new NotFoundResult(); 
        }
    }

    public void OnResourceExecuted(ResourceExecutedContext context) { }
}