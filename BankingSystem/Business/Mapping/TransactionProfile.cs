using AutoMapper;
using Business.Dtos.Transactions;
using Domain.Entities.Concretes;

namespace Business.Mapping;

public class TransactionProfile: Profile
{
    public TransactionProfile()
    {
        CreateMap<Transactions, TransactionResponseDto>();
    }
}