using AuthGuard.API.Entities;
using AuthGuard.API.Models.DTOs;
using AuthGuard.API.Models.Requests;
using AuthGuard.API.Repositories.Abstracts;
using AutoMapper;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace AuthGuard.API.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class HomeController : ControllerBase
    {
        private readonly IUserService _userService;
        private readonly IMapper _mapper;

        public HomeController(IUserService userService, IMapper mapper)
        {
            _userService = userService;
            _mapper = mapper;
        }

        [HttpPost]
        [AllowAnonymous]
        public IActionResult Authenticate(AuthenticateRequest request)
        {
            var response = _userService.Authenticate(request);
            return StatusCode(response.HttpStatusCode!.Value, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUsers()
        {
            var response = await _userService.GetUsersAsync().ConfigureAwait(false);
            return StatusCode(response.HttpStatusCode!.Value, response);
        }

        [HttpGet]
        public async Task<IActionResult> GetUser([FromQuery] int id)
        {
            var response = await _userService.GetUserAsync(id).ConfigureAwait(false);

            return StatusCode(response.HttpStatusCode!.Value, response);
        }

        [HttpPost]
        public async Task<IActionResult> PostUser([FromBody] UserDto request)
        {
            var response = await _userService.CreateUserAsync(_mapper.Map<User>(request)).ConfigureAwait(false)!;
            return StatusCode(response.HttpStatusCode!.Value, response);
        }

        [HttpPost]
        public async Task<IActionResult> ToggleUser([FromQuery] int id)
        {
            var user = await _userService.GetUserAsync(id);
            if (user == null)
                return StatusCode((int)user!.HttpStatusCode!, user);

            user.Model!.IsActive = !user.Model.IsActive;
            var response = await _userService.UpdateUserAsync(_mapper.Map<User>(user.Model));
            return StatusCode(response.HttpStatusCode!.Value, response);
        }

        [HttpPatch]
        public  async Task<IActionResult> PatchUser([FromQuery] int id, [FromBody] UserDto request)
        {
            request.ID = id;
            var response = await _userService.UpdateUserAsync(_mapper.Map<User>(request));
            return StatusCode(response.HttpStatusCode!.Value, response);
        }
    }
}