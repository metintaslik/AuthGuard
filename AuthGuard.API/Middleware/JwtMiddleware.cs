using AuthGuard.API.Models.Responses;
using AuthGuard.API.Repositories.Abstracts;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace AuthGuard.API.Middleware
{
    public class JwtMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly AppSettings _appSettings;
        private readonly ILogger _logger;
        private readonly IMemoryCache _cache;

        public JwtMiddleware(RequestDelegate next, IOptions<AppSettings> options, ILogger logger, IMemoryCache cache)
        {
            _next = next;
            _appSettings = options.Value;
            _logger = logger;
            _cache = cache;
        }

        public async Task Invoke(HttpContext context)
        {
            try
            {
                string? token = context.Request.Headers["Authorization"].FirstOrDefault()?.Split(" ").Last();
                if (!string.IsNullOrEmpty(token))
                    AttachUserToContext(context, token);

                await _next(context);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, ex.Message, DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;
                await context.Response.WriteAsJsonAsync(new BaseResponse<object>(true, errorCode: (int)ErrorType.UnExpectedErrorOccured, errorDetail: ErrorType.UnExpectedErrorOccured.ToString()))
                    .ConfigureAwait(false);
            }
        }

        private void AttachUserToContext(HttpContext context, string token)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);

            bool canRead = tokenHandler.CanReadToken(token);
            var secToken = tokenHandler.ReadToken(token);
            var jwtSecToken = tokenHandler.ReadJwtToken(token);
            if (canRead)
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    ClockSkew = TimeSpan.Zero,
                    RequireExpirationTime = true,
                }, out SecurityToken validatedToken);
            }
            else
            {
                _logger.LogError("Unauthorized access.", DateTime.Now.ToString("dd.MM.yyyy HH:mm:ss"));
                context.Response.Clear();
                context.Response.ContentType = "application/json";
                context.Response.StatusCode = (int)HttpStatusCode.Unauthorized;
            }
        }
    }
}