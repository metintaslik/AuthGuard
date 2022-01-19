using AuthGuard.Security.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using System.Net.Http.Headers;
using System.Security.Claims;

namespace AuthGuard.Security.Helpers
{
    public class AuthorizeAspectAttribute : ActionFilterAttribute
    {
        public async override Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
        {
            string token = context.HttpContext.Request.Headers["Authorization"];

            var cache = context.HttpContext.RequestServices.GetRequiredService<IMemoryCache>();
            var user = context.HttpContext.User;
            if (string.IsNullOrEmpty(token) && user != null && user.Identity!.IsAuthenticated)
                token = cache.Get<ApiAuthorizeResponse>(user.Claims.FirstOrDefault(x => x.Type == "urn:github:login")!.Value)!.Token;

            if (string.IsNullOrEmpty(token))
            {
                context.Result = new RedirectResult("api/Employee/Authorize");
                return;
            }

            await base.OnActionExecutionAsync(context, next).ConfigureAwait(false);
        }
    }
}