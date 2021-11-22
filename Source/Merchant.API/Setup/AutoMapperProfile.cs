using System.Linq;
using AutoMapper;
using Merchant.API.ViewModels;
using Merchant.CORE.Dtos;
using Merchant.CORE.EFModels;
using Merchant.CORE.EFObjects;
using Merchant.CORE.Models;

namespace Merchant.API.Setup
{
    public class AutoMapperProfile: Profile
    {
        public AutoMapperProfile()
        {
            CreateMap<ValidationResult, ValidationResultViewModel>();
            CreateMap<ProductDto, Product>()
                .ForMember(d => d.OrderItems, o => o.Ignore())
                .ReverseMap();
            CreateMap<ProductViewModel, ProductDto>().ReverseMap();
            CreateMap<CORE.EFModels.Order, OrderDto>()
                .ForMember(d => d.UserName,
                    o => o.MapFrom(s => s.User == null ? s.UserId.ToString() : s.User.Name.ToLower()))
                .ForMember(d => d.UserAddress, o => o.MapFrom(s => s.User == null ? "|||||" : s.User.Address))
                .ForMember(d => d.UserEmail, o => o.MapFrom(s => s.User == null ? "" : s.User.Email.ToLower()))
                .ForMember(d => d.UserFirstName, o => o.MapFrom(s => s.User == null ? "" : s.User.FirstName))
                .ForMember(d => d.UserLastName, o => o.MapFrom(s => s.User == null ? "" : s.User.LastName));
            CreateMap<OrderDto, CORE.EFModels.Order>()
                .ForMember(d => d.User, o => o.Ignore());
            CreateMap<OrderViewModel, OrderDto>().ReverseMap();
            CreateMap<OrderItemDto, OrderItem>()
                .ForMember(d => d.OrderId, o => o.Ignore())
                .ForMember(d => d.Order, o => o.Ignore())
                .ForMember(d => d.Product, o => o.Ignore())
                .ReverseMap();
            CreateMap<OrderItemViewModel, OrderItemDto>().ReverseMap();
            CreateMap<User, UserIdentity>()
                .ForMember(d => d.Name, o => o.MapFrom(source => source.NormalizedUserName.ToLower()))
                .ForMember(d => d.Roles, o => o.Ignore());
            CreateMap<User, UserIdentityViewModel>()
                .ForMember(d => d.Name, o => o.MapFrom(source => source.NormalizedUserName.ToLower()))
                .ForMember(d => d.Roles, o => o.Ignore());
            CreateMap<UserIdentity, UserIdentityViewModel>()
                .ForMember(d => d.Roles, o => o.MapFrom(source => source.Roles.Select(x => x.ToString())));
        }
    }
}
