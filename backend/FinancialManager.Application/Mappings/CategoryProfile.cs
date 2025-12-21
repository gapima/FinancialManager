using AutoMapper;
using FinancialManager.Application.Contracts.Category;
using FinancialManager.Domain.Entities;
using FinancialManager.Domain.Enums;

namespace FinancialManager.Application.Mapping;

public sealed class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryResponseDto>()
                   .ForMember(d => d.Purpose, opt => opt.MapFrom(s => (int)s.Purpose));

        CreateMap<CategoryCreateDto, Category>()
            .ForMember(d => d.Purpose, opt => opt.MapFrom(s => (CategoryPurpose)s.Purpose));

        CreateMap<CategoryUpdateDto, Category>()
            .ForMember(d => d.Purpose, opt => opt.MapFrom(s => (CategoryPurpose)s.Purpose));
    }
}
