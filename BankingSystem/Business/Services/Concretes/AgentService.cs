using Business.Wrappers;
using DataAccess.Repositories.Interfaces;
using Domain.Entities.Concretes;

namespace Business.Services;

public class AgentService(IRepository<ATM, Guid> atmRepository): IAgentService
{
    public async Task<Response<PagedResponse<ATM>>> GetAllAtms(int threshold, int limit)
    {
        var atms =await  atmRepository.GetByAsync(
            threshold,
            limit
        );
        var totalAtms = await atmRepository.CountAsync();
        var pagination = PagedResponse<ATM>.Create(atms, totalAtms, threshold, limit);
        return Response<PagedResponse<ATM>>.Success(pagination);
    }

    public async Task<Response<ATM>> GetAtmById(Guid atmId)
    {
        var atm = await atmRepository.GetByIdAsync(atmId);
    
        if (atm == null)
        {
            return Response<ATM>.Failure("Account not found");
        }

        return Response<ATM>.Success(
            atm
        );
    }
}