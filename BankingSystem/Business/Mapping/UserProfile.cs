using AutoMapper;
using Business.Dtos;
using Domain.Entities.Concretes;

namespace Business.Mapping
{
    public class UserProfile : Profile
    {
        public UserProfile()
        {
            CreateMap<User, SimpleUserDto>();
            CreateMap<SimpleUserDto, User>();
            CreateMap<CreateUserDto, User>();
        }
    }
}