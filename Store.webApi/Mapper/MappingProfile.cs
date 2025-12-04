using AutoMapper;
using Store.DataAccess.Entities;
using Store.WebApi.Dtos.Address;
using Store.WebApi.Dtos.Cart;
using Store.WebApi.Dtos.CartItem;
using Store.WebApi.Dtos.Category;
using Store.WebApi.Dtos.Order;
using Store.WebApi.Dtos.OrderItem;
using Store.WebApi.Dtos.Payment;
using Store.WebApi.Dtos.Product;
using Store.WebApi.Dtos.Review;
using Store.WebApi.Dtos.User;

namespace Store.WebApi.Mapping
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            // User mappings
            CreateMap<ApplicationUser, UserDto>()
                .ForMember(dest => dest.Roles, opt => opt.Ignore());

            CreateMap<UserRegisterDto, ApplicationUser>()
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src => src.Email));

            CreateMap<UserUpdateDto, ApplicationUser>()
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Address mappings
            CreateMap<Address, AddressDto>();
            CreateMap<AddressCreateDto, Address>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<AddressUpdateDto, Address>()
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Category mappings
            CreateMap<Category, CategoryDto>()
                .ForMember(dest => dest.ProductCount, opt => opt.MapFrom(src => src.Products!.Count));
            CreateMap<CategoryCreateDto, Category>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<CategoryUpdateDto, Category>()
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Product mappings
            CreateMap<Product, ProductDto>()
                .ForMember(dest => dest.CategoryName, opt => opt.MapFrom(src => src.Category!.Name));
            CreateMap<ProductCreateDto, Product>()
                .ForMember(dest => dest.CreatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));
            CreateMap<ProductUpdateDto, Product>()
                .ForMember(dest => dest.UpdatedDate, opt => opt.MapFrom(_ => DateTime.UtcNow));

            // Cart mappings
            CreateMap<Cart, CartDto>()
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src =>
                    src.CartItems!.Sum(ci => ci.Quantity * ci.PriceSnapshot)))
                .ForMember(dest => dest.TotalItems, opt => opt.MapFrom(src =>
                    src.CartItems!.Sum(ci => ci.Quantity)));

            CreateMap<CartItem, CartItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
                .ForMember(dest => dest.ProductImage, opt => opt.MapFrom(src => src.Product!.ImageUrl));

            // Order mappings
            CreateMap<Order, OrderDto>()
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => $"ORD-{src.Id:00000}"))
                .ForMember(dest => dest.Subtotal, opt => opt.MapFrom(src =>
                    src.OrderItems!.Sum(oi => oi.Quantity * oi.UnitPriceSnapshot)));

            CreateMap<OrderItem, OrderItemDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.ProductNameSnapshot))
                .ForMember(dest => dest.ProductImage, opt => opt.Ignore());

            // Payment mappings
            CreateMap<Payment, PaymentDto>()
                .ForMember(dest => dest.OrderNumber, opt => opt.MapFrom(src => $"ORD-{src.OrderId:00000}"));

            // Review mappings
            CreateMap<Review, ReviewDto>()
                .ForMember(dest => dest.ProductName, opt => opt.MapFrom(src => src.Product!.Name))
                .ForMember(dest => dest.UserName, opt => opt.MapFrom(src =>
                    $"{src.User!.FirstName} {src.User.LastName}".Trim()));
        }
    }
}