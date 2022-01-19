using AuthGuard.API.Entities;
using AuthGuard.API.Models.DTOs;
using AutoMapper;

namespace AuthGuard.API.Models.Profiles
{
    public class AutoMapperProfiles : Profile
    {
        public AutoMapperProfiles()
        {
            CreateMap<User, UserDto>().ReverseMap();
        }
    }
}