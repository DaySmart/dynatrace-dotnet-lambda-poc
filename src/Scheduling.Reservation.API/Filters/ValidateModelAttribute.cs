using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;

namespace Scheduling.Reservation.Filters;

public class ValidateModelAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext actionContext)
    {
        if (!actionContext.ModelState.IsValid)
        {
            actionContext.Result = new BadRequestObjectResult(actionContext.ModelState);
        }
    }
}
