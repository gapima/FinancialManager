using AutoMapper;
using FinancialManager.Application.Contracts.Transaction;
using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Mappings;

public sealed class TransactionProfile : Profile
{
    public TransactionProfile()
    {
        CreateMap<Transaction, TransactionResponseDto>();

        CreateMap<TransactionCreateDto, Transaction>()
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description.Trim()));

        CreateMap<TransactionUpdateDto, Transaction>()
            .ForMember(d => d.Description, opt => opt.MapFrom(s => s.Description.Trim()));
    }
}
