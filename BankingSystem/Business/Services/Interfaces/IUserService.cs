using Business.Dtos;
using Business.Wrappers;

namespace Business.Services;

public interface IUserService
{
    Task<Response<Guid>> Create(CreateUserDto createUserDto);
    Task<Response<PagedResponse<SimpleUserDto>>> GetAll(int threshold, int limit);
    Task<Response<UserDto>> GetById(Guid userId);
}