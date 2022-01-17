using AuthGuard.API.Models.Requests;
using AuthGuard.API.Models.Responses;

namespace AuthGuard.API.Repositories.Abstracts
{
    public interface IUserService
    {
        BaseResponse<AuthenticateResponse> Authenticate(AuthenticateRequest model);
    }
}
