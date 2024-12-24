using App.Application;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace CleanApp.API.Filters;

public class FluentValidationFilter : IAsyncActionFilter
{
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        if (!context.ModelState.IsValid)
        {
            var errors = context.ModelState.Values
                .SelectMany(e => e.Errors)
                .Select(x => x.ErrorMessage)
                .ToList();

            var resultModel = ApplicationResult.Fail(errors);

            context.Result = new BadRequestObjectResult(resultModel);
            return;
        }

        await next();
    }
}
