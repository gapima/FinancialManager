using AutoMapper;
using FinancialManager.Application.Contracts.Category;
using FinancialManager.Domain.Entities;

namespace FinancialManager.Application.Mapping;

public sealed class CategoryProfile : Profile
{
    public CategoryProfile()
    {
        CreateMap<Category, CategoryResponseDto>();
        CreateMap<CategoryCreateDto, Category>();
        CreateMap<CategoryUpdateDto, Category>();
    }
}
