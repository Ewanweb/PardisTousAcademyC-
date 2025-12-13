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

            // تبدیل User به UserResource (بدون Courses برای جلوگیری از circular reference)
            CreateMap<User, UserResource>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Roles, opt => opt.Ignore()) // Roles رو جداگانه handle کن
                .ReverseMap();

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

            // تبدیل CourseSchedule به CourseScheduleDto
            CreateMap<CourseSchedule, CourseScheduleDto>()
                .ForMember(dest => dest.DayName, opt => opt.MapFrom(src => src.GetDayName()))
                .ForMember(dest => dest.TimeRange, opt => opt.MapFrom(src => $"{src.StartTime:HH:mm}-{src.EndTime:HH:mm}"))
                .ForMember(dest => dest.FullScheduleText, opt => opt.MapFrom(src => src.GetFullScheduleText()))
                .ForMember(dest => dest.RemainingCapacity, opt => opt.MapFrom(src => src.RemainingCapacity))
                .ForMember(dest => dest.HasCapacity, opt => opt.MapFrom(src => src.HasCapacity));

            // تبدیل User به InstructorBasicDto (بدون Courses)
            CreateMap<User, InstructorBasicDto>()
                .ForMember(dest => dest.FullName, opt => opt.MapFrom(src => src.FullName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Mobile, opt => opt.MapFrom(src => src.Mobile));

            CreateMap<UserCourse, CourseResource>()
                // الف) ابتدا تمام اطلاعات را از "Course" بردار (Title, Price, Image, ...)
                // این کار باعث می‌شود از کانفیگ "Course -> CourseResource" استفاده کند
                .IncludeMembers(s => s.Course)

                // ب) سپس فیلدهای اختصاصی را از "UserCourse" بازنویسی کن
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.CourseId)) // آی‌دی دوره را بگذار نه آی‌دی جدول واسط
                .ForMember(dest => dest.IsCompleted, opt => opt.MapFrom(src => src.IsCompleted)) // وضعیت تکمیل دانشجو
                .ForMember(dest => dest.IsStarted, opt => opt.MapFrom(src => true)) // چون ثبت‌نام کرده، یعنی شروع کرده
                .ForMember(dest => dest.CreatedAt, opt => opt.MapFrom(src => src.Course.CreatedAt)); // تاریخ ایجاد دوره (نه تاریخ ثبت‌نام)
            // اگر خواستید تاریخ ثبت‌نام را برگردانید، باید یک فیلد EnrolledAt به CourseResource اضافه کنید

            // تبدیل Course به CourseResource
            CreateMap<Course, CourseResource>()
                .ForMember(dest => dest.Sections, opt => opt.MapFrom(src => src.Sections))
                .ForMember(dest => dest.Schedules, opt => opt.MapFrom(src => src.Schedules))
                .ForMember(dest => dest.Status, opt => opt.MapFrom(src => src.Status.ToString()))
                .ForMember(dest => dest.Type, opt => opt.MapFrom(src => src.Type.ToString()))
                .ForMember(dest => dest.Instructor, opt => opt.MapFrom(src => src.Instructor))
                .ReverseMap();
        }
    }
}
