using AutoMapper;
using DataAccess.Entities;
using Shared.Dto;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Business.Automapper
{
    public class AutoMapperProfile : Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<UserDto, User>();
            CreateMap<User, UserDto>();
            CreateMap<User, AuthenticateResponseDto>();            
            CreateMap<RegisterRequestDto, User>().ReverseMap();
            CreateMap<UpdatePasswordDto, User>();                        
        }
    }
}
