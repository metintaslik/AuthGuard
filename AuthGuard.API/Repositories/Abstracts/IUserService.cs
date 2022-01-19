using AuthGuard.API.Entities;
using AuthGuard.API.Models.Requests;
using AuthGuard.API.Models.Responses;

namespace AuthGuard.API.Repositories.Abstracts
{
    public interface IUserService
    {
        BaseResponse<AuthenticateResponse> Authenticate(AuthenticateRequest model);
        BaseResponse<User> GetUser(int id);
        Task<BaseResponse<IEnumerable<User>>> GetUsersAsync(Func<bool, User>? expression = null);
        BaseResponse<User> CreateUser(User entity);
        BaseResponse<User> UpdateUser(User entity);
        BaseResponse<object> DeleteUser(User entity);
    }
}