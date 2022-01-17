using AuthGuard.API.Models.Requests;
using AuthGuard.API.Repositories.Abstracts;
using Microsoft.AspNetCore.Mvc;

namespace AuthGuard.API.Controllers
{
    [Route("api/[controller]")]
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
            return StatusCode(response.HttpStatusCode, response);
        }
    }
}