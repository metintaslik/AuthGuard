﻿using AuthGuard.API.Middleware;
using AuthGuard.API.Models.Requests;
using AuthGuard.API.Models.Responses;
using AuthGuard.API.Repositories.Abstracts;
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

        public UserService(IOptions<RoofAuthGuardSettings> options, AppSettings appSettings, IMemoryCache cache)
        {
            _appSettings = appSettings;
            roofAuthGuardSettings = options.Value;
            _cache = cache;
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

        private string GenerateJwtToken(DateTime expire)
        {
            JwtSecurityTokenHandler tokenHandler = new();
            byte[]? key = Encoding.ASCII.GetBytes(_appSettings.Secret);
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