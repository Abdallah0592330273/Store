using AutoMapper;
using Domain.Entities;
using Store.Api.DTOs;
using Store.Api.DTOs.CartItems;
using Store.Api.DTOs.Payments;
using Store.Api.DTOs.Products;
using Store.Api.DTOs.Reviws;
using Store.Api.DTOs.Users;


namespace Store.Api.Mappings
{
    public class Mapper : Profile
    {
        public Mapper() {
            CreateMap<User, UserDto>().ReverseMap();
            CreateMap<User, CreateUserDto>().ReverseMap();
            CreateMap<User, UpdateUserDto>().ReverseMap();
            CreateMap<CartItem, CartItemDto>().ReverseMap();
            CreateMap<CartItem, CreateCartItemDto>().ReverseMap();
            CreateMap<CartItem, UpdateCartItemDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<Order, OrderDto>().ReverseMap();
            CreateMap<OrderItem, OrderItemDto>().ReverseMap();
            CreateMap<OrderItem, CreateOrderItemDto>().ReverseMap();
            CreateMap<Payment, PaymentDto>().ReverseMap();
            CreateMap<Payment, CreatePaymentDto>().ReverseMap();
            CreateMap<Payment, UpdatePaymentDto>().ReverseMap();
            CreateMap<Product, ProductDto>().ReverseMap();
            CreateMap<Product, CreateProductDto>().ReverseMap();
            CreateMap<Product, UpdateProductDto>().ReverseMap();
            CreateMap<Reviw, ReviewDto>().ReverseMap();
            CreateMap<Reviw, CreateReviwDto>().ReverseMap();
            CreateMap<Reviw, UpdateReviwDto>().ReverseMap();
        }

    }
}
