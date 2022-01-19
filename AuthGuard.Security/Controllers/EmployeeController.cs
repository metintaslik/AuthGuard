using AuthGuard.Security.Helpers;
using AuthGuard.Security.Models.Response;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
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
        private readonly IMemoryCache _cache;

        public EmployeeController(IHttpClientFactory factory, IOptions<RoofApiSettings> options, IMemoryCache cache)
        {
            _httpClient = factory.CreateClient();
            roofApiSettings = options.Value;
            _cache = cache;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Authorize()
        {
            string url = roofApiSettings.BaseUrl + "/api/Home/Authenticate";
            var responseMessage = await _httpClient.PostAsJsonAsync(url, roofApiSettings).ConfigureAwait(false);
            if (responseMessage.StatusCode != HttpStatusCode.OK)
                return StatusCode((int)responseMessage!.StatusCode, await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false));

            var responseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            string expire = JObject.Parse(responseContent)["model"]["expireTime"]!.ToString();
            ApiBaseResponse<ApiAuthorizeResponse> response = JsonConvert.DeserializeObject<ApiBaseResponse<ApiAuthorizeResponse>>(responseContent, new JsonSerializerSettings { DateParseHandling = DateParseHandling.DateTime })!;
            response.Model!.Expire = DateTime.ParseExact(expire, "dd.MM.yyyy HH:mm:ss", null);
            string username = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "urn:github:login")!.Value;
            _cache.Set(username, response.Model);
            return StatusCode((int)responseMessage.StatusCode, response);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> GetUsers()
        {
            string url = roofApiSettings.BaseUrl + "/api/Home/GetUsers";
            string username = HttpContext.User.Claims.FirstOrDefault(x => x.Type == "urn:github:login")!.Value;
            _cache.TryGetValue(username, out ApiAuthorizeResponse authorize);

            _httpClient.DefaultRequestHeaders.Add("Authorization", authorize.Token);

            var responseMessage = await _httpClient.GetAsync(url, CancellationToken.None).ConfigureAwait(false);
            if (responseMessage.StatusCode != HttpStatusCode.OK)
                return StatusCode((int)responseMessage!.StatusCode, await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false));

            var responseContent = await responseMessage.Content.ReadAsStringAsync().ConfigureAwait(false);
            ApiBaseResponse<List<ApiUser>> response = JsonConvert.DeserializeObject<ApiBaseResponse<List<ApiUser>>>(responseContent)!;
            return StatusCode((int)responseMessage.StatusCode, response);
        }
    }
}