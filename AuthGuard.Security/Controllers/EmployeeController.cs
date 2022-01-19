using AuthGuard.Security.Helpers;
using AuthGuard.Security.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Newtonsoft.Json.Linq;
using System.Net;
using System.Security.Claims;

namespace AuthGuard.Security.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class EmployeeController : ControllerBase
    {
        private readonly HttpClient _httpClient;
        private readonly RoofApiSettings roofApiSettings;

        public EmployeeController(IHttpClientFactory factory, IOptions<RoofApiSettings> options)
        {
            _httpClient = factory.CreateClient();
            roofApiSettings = options.Value;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Authorize()
        {
            string url = roofApiSettings.BaseUrl + "/api/Auth/Authenticate";
            var responseMessage = await _httpClient.PostAsJsonAsync(url, roofApiSettings).ConfigureAwait(false);
            if (responseMessage.StatusCode != HttpStatusCode.OK)
                return StatusCode((int)responseMessage!.StatusCode, await responseMessage.Content.ReadAsStringAsync());

            var responseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            ApiAuthorizeResponse response = JObject.Parse(responseContent)["model"]!.ToObject<ApiAuthorizeResponse>()!;

            var claims = new List<Claim> { new Claim("ApiToken", response.Token), new Claim("Expire", response.Expire.ToString("dd.MM.yyyy HH:mm:ss")) };
            HttpContext.User.AddIdentity(new ClaimsIdentity(claims, authenticationType: "AuthGuard.Api"));

            return StatusCode(200);
        }
    }
}