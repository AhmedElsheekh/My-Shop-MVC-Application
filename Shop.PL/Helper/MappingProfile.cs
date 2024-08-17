using AutoMapper;
using Shop.DAL.Entities;
using Shop.PL.ViewModels;

namespace Shop.PL.Helper
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<Category, CategoryViewModel>().ReverseMap();

            CreateMap<Product, ProductDetailsVM>()
                .ForMember(d => d.CategoryName, s => s.MapFrom(p => p.Category.Name));

            CreateMap<ProductCreateVM, Product>();

            CreateMap<Product, ProductUpdateVM>().ReverseMap();

            CreateMap<Product, BasketItemVM>();

            CreateMap<ApplicationUser, UserDetailsVM>();

            CreateMap<OrderHeader, OrderHeaderVM>()
                .ForMember(d => d.Email, s => s.MapFrom(o => o.ApplicationUser.Email));

            CreateMap<OrderDetail, OrderDetailVM>()
                .ForMember(d => d.ProductDetails, s => s.MapFrom(o => o.Product));
                

        }
    }
}
