using AutoMapper;
using Mall.Model;
using Mall.Model.Dtos;
using Mall.Model.Entity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mall.WebApi.Profiles
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<UserInfo, UserDto>()
                .ForMember(
                //需要取数据的成员
                dest => dest.Name,
                //数据来源方              
                opt => opt.MapFrom(src => src.UserName))
                .ForMember(
                 dest => dest.Roles,
                 opt => opt.MapFrom(src => src.Roles));
        }
    }
}
