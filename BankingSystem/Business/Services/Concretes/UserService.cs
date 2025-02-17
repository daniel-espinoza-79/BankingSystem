using AutoMapper;
using Business.Dtos;
using Business.Wrappers;
using DataAccess.Repositories.Interfaces;
using Domain.Entities.Concretes;

namespace Business.Services;

public class UserService(IRepository<User,Guid> userRepository, IMapper mapper, IRepository<BankAccount, Guid> bankAccountRepository, IImageService imageService): IUserService
{
    public async Task<Response<Guid>> Create(CreateUserDto createUserDto)
    {
        var existUser = userRepository.FindOneAsync(u => u.DocumentId == createUserDto.DocumentId);
        if (existUser == null)
        {
            throw new Exception($"User with id {createUserDto.DocumentId} already exists");
        }
        var user = mapper.Map<CreateUserDto, User>(createUserDto);
        await userRepository.CreateAsync(user);
        return Response<Guid>.Success(user.Id);
    }

    public async Task<Response<PagedResponse<SimpleUserDto>>> GetAll(int threshold, int limit)
    {
        var users = await  userRepository.GetByAsync(
            threshold,
            limit
        );
        var userDtos = mapper.Map<List<SimpleUserDto>>(users);
        var totalUsers = await userRepository.CountAsync();
        var pagination = PagedResponse<SimpleUserDto>.Create(userDtos, totalUsers, threshold, limit);
        return Response<PagedResponse<SimpleUserDto>>.Success(pagination);
    }

    public async Task<Response<UserDto>> GetById(Guid userId)
    {
        var user = await userRepository
            .GetByIdAsync(userId);

        if (user == null || !user.IsActive)
        {
            throw new KeyNotFoundException("User not found");
        }
        
        var faceBase64 = await imageService.TryToGetFaceImageAsync(user);
        var userDto = new UserDto()
        {
            Id = user.Id,
            Name = user.FullName,
            FaceBase64 = faceBase64,
            Accounts = await GetUserAccounts(user)
        };
        return Response<UserDto>.Success(userDto);
    }
    
    private async Task<List<AccountDto>> GetUserAccounts(User user)
    {
        var accounts = await bankAccountRepository.FindAsync((account) => account.UserId == user.Id);

        return [.. accounts.Select(
            account => new AccountDto()
            {
                Id = account.Id,
                AccountHolder = user.FullName??"",
                Balance = account.Balance
            }
        )];
    }
}