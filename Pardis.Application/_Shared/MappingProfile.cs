using AutoMapper;
using Pardis.Domain.Categories;
using Pardis.Domain.Courses;
using Pardis.Domain.Seo;
using Pardis.Domain.Users;
using System;
using System.Collections.Generic;
using System.Text;
using Pardis.Domain.Dto.Categories;
using Pardis.Domain.Dto.Courses;
using Pardis.Domain.Dto.Users;
using static Pardis.Domain.Dto.Dtos;

namespace Pardis.Application._Shared
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // تبدیل SeoMetadata به SeoDto
            CreateMap<SeoMetadata, SeoDto>().ReverseMap();

            // تبدیل User به UserResource
            CreateMap<User, UserResource>()
                            .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName)).ReverseMap();

            // تبدیل Category به CategoryResource
            CreateMap<Category, CategoryResource>()
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.IsActive, opt => opt.MapFrom(src => src.IsActive))
                .ReverseMap(); 

            CreateMap<CategoryWithCountDto, CategoryResource>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Category.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Category.Title))
                .ForMember(dest => dest.CoursesCount, opt => opt.MapFrom(src => src.CoursesCount))
                .ForMember(dest => dest.Slug, opt => opt.MapFrom(src => src.Category.Slug))
                .ForMember(dest => dest.Seo, opt => opt.MapFrom(src => src.Category.Seo)).ReverseMap();
            // Ignore کردن Children چون در این DTO لود نشده‌اند

            // تبدیل Course به CourseResource
            CreateMap<CourseSection, CourseSectionDto>().ReverseMap();

            // تبدیل Course به CourseResource
            CreateMap<Course, CourseResource>()
                .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections)) // این خط حالا کار می‌کند
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ReverseMap();
        }
    }
}
