using AutoMapper;
using FinancialManager.Application.Contracts.Pessoa;
using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Mappings;

public sealed class PessoaProfile : Profile
{
    public PessoaProfile()
    {
        CreateMap<Pessoa, PessoaResponseDto>();

        CreateMap<PessoaCreateDto, Pessoa>()
            .ForMember(d => d.Nome, opt => opt.MapFrom(s => s.Nome.Trim()));

        CreateMap<PessoaUpdateDto, Pessoa>()
            .ForMember(d => d.Nome, opt => opt.MapFrom(s => s.Nome.Trim()));
    }
}
    