using Business.Dtos;
using Business.Wrappers;
using Domain.Entities.Concretes;

namespace Business.Services;

public interface IAgentService
{
    Task<Response<PagedResponse<ATM>>> GetAllAtms(int threshold, int limit);
    Task<Response<ATM>> GetAtmById(Guid userId);
}