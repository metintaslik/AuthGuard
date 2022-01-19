using AuthGuard.API.Data;
using AuthGuard.API.Entities;
using AuthGuard.API.Middleware;
using AuthGuard.API.Models.DTOs;
using AuthGuard.API.Models.Requests;
using AuthGuard.API.Models.Responses;
using AuthGuard.API.Repositories.Abstracts;
using AutoMapper;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Net;
using System.Text;

namespace AuthGuard.API.Repositories.Concretes
{
    public class UserService : IUserService
    {
        private readonly AppSettings _appSettings;
        private readonly RoofAuthGuardSettings roofAuthGuardSettings;
        private readonly IMemoryCache _cache;
        private readonly ApplicationDBContext dBContext;
        private readonly IMapper _mapper;

        public UserService(IOptions<AppSettings> appSettingsOptions, IOptions<RoofAuthGuardSettings> authGuardOptions, IMemoryCache cache, ApplicationDBContext dBContext, IMapper mapper)
        {
            _appSettings = appSettingsOptions.Value;
            roofAuthGuardSettings = authGuardOptions.Value;
            _cache = cache;
            this.dBContext = dBContext;
            _mapper = mapper;
        }

        public BaseResponse<AuthenticateResponse> Authenticate(AuthenticateRequest model)
        {
            if (model == null)
                return new BaseResponse<AuthenticateResponse>(true, (int)HttpStatusCode.BadRequest, (int)ErrorType.ModelCannotBeLeftBlank, ErrorType.ModelCannotBeLeftBlank.ToString());

            if (roofAuthGuardSettings.ClientID != model.ClientID || roofAuthGuardSettings.ClientSecret != model.ClientSecret)
                return new BaseResponse<AuthenticateResponse>(true, (int)HttpStatusCode.Forbidden, (int)ErrorType.Forbidden, ErrorType.Forbidden.ToString());

            DateTime expire = DateTime.Now.AddHours(1);
            string token = GenerateJwtToken(expire);

            return new BaseResponse<AuthenticateResponse>(false, (int)HttpStatusCode.OK, model: new AuthenticateResponse { Token = token, ExpireTime = expire });
        }

        public async Task<BaseResponse<UserDto>> CreateUserAsync(User entity)
        {
            await dBContext.Users.AddAsync(entity).ConfigureAwait(false);
            int affected = await dBContext.SaveChangesAsync().ConfigureAwait(false);
            if (affected == 0)
                return new BaseResponse<UserDto>(true, (int)HttpStatusCode.BadRequest, (int)ErrorType.FailedToUserCreated, ErrorType.FailedToUserCreated.ToString());

            return new BaseResponse<UserDto>(false, (int)HttpStatusCode.OK, model: _mapper.Map<UserDto>(entity));
        }

        public async Task<BaseResponse<object>> DeleteUserAsync(User entity)
        {
            var user = await dBContext.Users.FirstOrDefaultAsync(x => x.ID == entity.ID).ConfigureAwait(false);
            if (user == null)
                return new BaseResponse<object>(true, (int)HttpStatusCode.BadRequest, (int)ErrorType.UserNotFound, ErrorType.UserNotFound.ToString(), false);

            dBContext.Users.Remove(user);
            int affected = await dBContext.SaveChangesAsync().ConfigureAwait(false);
            if (affected == 0)
                return new BaseResponse<object>(true, (int)HttpStatusCode.BadRequest, (int)ErrorType.FailedToUserDelete, ErrorType.FailedToUserDelete.ToString(), false);

            return new BaseResponse<object>(false, (int)HttpStatusCode.OK, model: true);
        }

        public async Task<BaseResponse<UserDto>> GetUserAsync(int id)
        {
            var user = await dBContext.Users.FirstOrDefaultAsync(x => x.ID == id).ConfigureAwait(false);
            return new BaseResponse<UserDto>(false, (int)HttpStatusCode.OK, model: _mapper.Map<UserDto>(user));
        }

        public async Task<BaseResponse<IEnumerable<UserDto>>> GetUsersAsync(Func<bool, User>? expression = null)
                => new BaseResponse<IEnumerable<UserDto>>(false, httpStatusCode: (int)HttpStatusCode.OK, model: _mapper.Map<List<UserDto>>(await dBContext.Users.ToListAsync().ConfigureAwait(false)));

        public async Task<BaseResponse<UserDto>> UpdateUserAsync(User entity)
        {
            var user = await dBContext.Users.FirstOrDefaultAsync(x => x.ID == entity.ID).ConfigureAwait(false);
            if (user == null)
                return new BaseResponse<UserDto>(true, (int)HttpStatusCode.BadRequest, (int)ErrorType.UserNotFound, ErrorType.UserNotFound.ToString());

            dBContext.Entry(user).CurrentValues.SetValues(entity);
            int affected = await dBContext.SaveChangesAsync();
            if (affected == 0)
                return new BaseResponse<UserDto>(true, (int)HttpStatusCode.BadRequest, (int)ErrorType.FailedToUserUpdate, ErrorType.FailedToUserUpdate.ToString());

            return new BaseResponse<UserDto>(false, (int)HttpStatusCode.OK, model: _mapper.Map<UserDto>(entity));
        }

        private string GenerateJwtToken(DateTime expire)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[]? key = Encoding.UTF8.GetBytes(_appSettings.Secret);
            SecurityTokenDescriptor tokenDescriptor = new()
            {
                Expires = expire,
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            SecurityToken securityToken = tokenHandler.CreateToken(tokenDescriptor);
            string jwtToken = tokenHandler.WriteToken(securityToken);
            _cache.Set(jwtToken, expire);

            return jwtToken;
        }
    }
}