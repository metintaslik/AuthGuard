using AuthGuard.API.Entities;
using AuthGuard.API.Models.DTOs;
using AuthGuard.API.Models.Requests;
using AuthGuard.API.Models.Responses;

namespace AuthGuard.API.Repositories.Abstracts
{
    public interface IUserService
    {
        BaseResponse<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        Task<BaseResponse<UserDto>> GetUserAsync(int id);
        Task<BaseResponse<IEnumerable<UserDto>>> GetUsersAsync(Func<bool, User>? expression = null);
        Task<BaseResponse<UserDto>> CreateUserAsync(User entity);
        Task<BaseResponse<UserDto>> UpdateUserAsync(User entity);
        Task<BaseResponse<object>> DeleteUserAsync(User entity);
    }
}