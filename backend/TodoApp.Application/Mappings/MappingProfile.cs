using AutoMapper;
using TodoApp.Application.DTOs;
using TodoApp.Domain.Entities;

namespace TodoApp.Application.Mappings;

public class MappingProfile : Profile
{
    public MappingProfile()
    {
        CreateMap<TodoItem, TodoDto>()
            .ForMember(d => d.IsOverdue, opt => opt.MapFrom(s => s.IsOverdue()));
    }
}
