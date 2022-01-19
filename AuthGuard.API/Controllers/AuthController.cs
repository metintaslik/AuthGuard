using AuthGuard.API.Models.Requests;
using AuthGuard.API.Repositories.Abstracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthGuard.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthController : ControllerBase
    {
        private readonly IUserService _userService;

        public AuthController(IUserService userService)
        {
            _userService = userService;
        }

        [HttpPost]
        public IActionResult Authenticate(AuthenticateRequest request)
        {
            var response = _userService.Authenticate(request);
            return StatusCode(response.HttpStatusCode!.Value, response);
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> GetUsersAsync()
        {
            return StatusCode(200, "");
        }
    }
}