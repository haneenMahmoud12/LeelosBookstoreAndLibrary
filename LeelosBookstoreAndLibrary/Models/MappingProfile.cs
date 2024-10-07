using AutoMapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class MappingProfile : Profile
    {
        public MappingProfile()
        {
            CreateMap<DataLayer.Book, Models.Book>()
            .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
            .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
            .ForMember(dest => dest.Price, opt => opt.MapFrom(src => (float)src.Price))
            .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
            .ForMember(dest => dest.PublisherId, opt => opt.MapFrom(src => src.PublisherId))
            .ForMember(dest => dest.NumberOfPages, opt => opt.MapFrom(src => src.NumberOfPages))
            .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
            .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
            .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
            .ForMember(dest => dest.DatePublished, opt => opt.MapFrom(src => src.DatePublished))
            .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre))
            .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => src.ImageData))
            .ForMember(dest => dest.ImageMimeType, opt => opt.MapFrom(src => src.ImageMimeType))
            .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
            .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher));

            CreateMap<Models.Book, DataLayer.Book>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Title, opt => opt.MapFrom(src => src.Title))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => (double)src.Price))
                .ForMember(dest => dest.AuthorId, opt => opt.MapFrom(src => src.AuthorId))
                .ForMember(dest => dest.PublisherId, opt => opt.MapFrom(src => src.PublisherId))
                .ForMember(dest => dest.NumberOfPages, opt => opt.MapFrom(src => src.NumberOfPages))
                .ForMember(dest => dest.Rating, opt => opt.MapFrom(src => src.Rating))
                .ForMember(dest => dest.StockQuantity, opt => opt.MapFrom(src => src.StockQuantity))
                .ForMember(dest => dest.Description, opt => opt.MapFrom(src => src.Description))
                .ForMember(dest => dest.DatePublished, opt => opt.MapFrom(src => src.DatePublished))
                .ForMember(dest => dest.Genre, opt => opt.MapFrom(src => src.Genre))
                .ForMember(dest => dest.ImageData, opt => opt.MapFrom(src => src.ImageData))
                .ForMember(dest => dest.ImageMimeType, opt => opt.MapFrom(src => src.ImageMimeType))
                .ForMember(dest => dest.Author, opt => opt.MapFrom(src => src.Author))
                .ForMember(dest => dest.Publisher, opt => opt.MapFrom(src => src.Publisher));

            CreateMap<DataLayer.Author, Models.Author>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => src.Biography))
                .ForMember(dest => dest.ImageMimeType, opt => opt.MapFrom(src => src.ImageMimeType))
                .ForMember(dest => dest.AuthorImage, opt => opt.MapFrom(src => src.AuthorImage));

            CreateMap<Models.Author, DataLayer.Author>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Biography, opt => opt.MapFrom(src => src.Biography))
                .ForMember(dest => dest.ImageMimeType, opt => opt.MapFrom(src => src.ImageMimeType))
                .ForMember(dest => dest.AuthorImage, opt => opt.MapFrom(src => src.AuthorImage));

            CreateMap<DataLayer.Publisher, Models.Publisher>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<Models.Publisher, DataLayer.Publisher>()
               .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
               .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.Name))
               .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address))
               .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
               .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<DataLayer.Borrow, Models.Borrow>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.BorrowDate, opt => opt.MapFrom(src => src.BorrowDate))
                .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.ReturnDate))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.IsReturned, opt => opt.MapFrom(src => src.IsReturned))
                .ForMember(dest => dest.borrowFee, opt => opt.MapFrom(src => src.BorrowFee))
                .ForMember(dest => dest.LateFee, opt => opt.MapFrom(src => src.LateFee))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book));

            CreateMap<Models.Borrow, DataLayer.Borrow>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.BorrowDate, opt => opt.MapFrom(src => src.BorrowDate))
                .ForMember(dest => dest.ReturnDate, opt => opt.MapFrom(src => src.ReturnDate))
                .ForMember(dest => dest.DueDate, opt => opt.MapFrom(src => src.DueDate))
                .ForMember(dest => dest.IsReturned, opt => opt.MapFrom(src => src.IsReturned))
                .ForMember(dest => dest.BorrowFee, opt => opt.MapFrom(src => src.borrowFee))
                .ForMember(dest => dest.LateFee, opt => opt.MapFrom(src => src.LateFee))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book));

            CreateMap<DataLayer.BorrowCart, Models.BorrowCart>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.BorrowCartItems, opt => opt.MapFrom(src => src.BorrowCartItems))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<Models.BorrowCart, DataLayer.BorrowCart>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.BorrowCartItems, opt => opt.MapFrom(src => src.BorrowCartItems))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<DataLayer.BorrowCartItem, Models.BorrowCartItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BorrowCartId, opt => opt.MapFrom(src => src.BorrowCartId))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.BorrowCart, opt => opt.MapFrom(src => src.BorrowCart))
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book));

            CreateMap<Models.BorrowCartItem, DataLayer.BorrowCartItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BorrowCartId, opt => opt.MapFrom(src => src.BorrowCartId))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.BorrowCart, opt => opt.MapFrom(src => src.BorrowCart))
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book));

            CreateMap<DataLayer.Order, Models.Order>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));

            CreateMap<Models.Order, DataLayer.Order>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.OrderDate, opt => opt.MapFrom(src => src.OrderDate))
                .ForMember(dest => dest.OrderItems, opt => opt.MapFrom(src => src.OrderItems))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User))
                .ForMember(dest => dest.TotalPrice, opt => opt.MapFrom(src => src.TotalPrice));

            CreateMap<DataLayer.OrderItem, Models.OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<Models.OrderItem, DataLayer.OrderItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.OrderId, opt => opt.MapFrom(src => src.OrderId))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Order, opt => opt.MapFrom(src => src.Order))
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<DataLayer.Address, Models.ShippingInfo>()
                .ForMember(dest => dest.Address, opt => opt.MapFrom(src => src.Address1))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<Models.ShippingInfo, DataLayer.Address>()
                .ForMember(dest => dest.Address1, opt => opt.MapFrom(src => src.Address))
                .ForMember(dest => dest.City, opt => opt.MapFrom(src => src.City))
                .ForMember(dest => dest.ZipCode, opt => opt.MapFrom(src => src.ZipCode))
                .ForMember(dest => dest.PhoneNumber, opt => opt.MapFrom(src => src.PhoneNumber));

            CreateMap<DataLayer.ShoppingCart, Models.ShoppingCart>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<Models.ShoppingCart, DataLayer.ShoppingCart>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.UserId, opt => opt.MapFrom(src => src.UserId))
                .ForMember(dest => dest.User, opt => opt.MapFrom(src => src.User));

            CreateMap<DataLayer.ShoppingCartItem, Models.ShoppingCartItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<Models.ShoppingCartItem, DataLayer.ShoppingCartItem>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.BookId, opt => opt.MapFrom(src => src.BookId))
                .ForMember(dest => dest.Quantity, opt => opt.MapFrom(src => src.Quantity))
                .ForMember(dest => dest.Book, opt => opt.MapFrom(src => src.Book))
                .ForMember(dest => dest.Price, opt => opt.MapFrom(src => src.Price));

            CreateMap<DataLayer.ShoppingCart_ShoppingCartItems, Models.ShoppingCartShoppingCartItem>()
            .ForMember(dest => dest.ShoppingCartId, opt => opt.MapFrom(src => src.ShoppingCartId))
            .ForMember(dest => dest.ShoppingCartItemId, opt => opt.MapFrom(src => src.ShoppingCartItemId))
            .ForMember(dest => dest.ShoppingCart, opt => opt.MapFrom(src => src.ShoppingCart))
            .ForMember(dest => dest.ShoppingCartItem, opt => opt.MapFrom(src => src.ShoppingCartItem));

            CreateMap<Models.ShoppingCartShoppingCartItem, DataLayer.ShoppingCart_ShoppingCartItems>()
                .ForMember(dest => dest.ShoppingCartId, opt => opt.MapFrom(src => src.ShoppingCartId))
                .ForMember(dest => dest.ShoppingCartItemId, opt => opt.MapFrom(src => src.ShoppingCartItemId))
                .ForMember(dest => dest.ShoppingCart, opt => opt.MapFrom(src => src.ShoppingCart))
                .ForMember(dest => dest.ShoppingCartItem, opt => opt.MapFrom(src => src.ShoppingCartItem));

            // Mapping for User
            CreateMap<DataLayer.User, Models.User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));

            CreateMap<Models.User, DataLayer.User>()
                .ForMember(dest => dest.Id, opt => opt.MapFrom(src => src.Id))
                .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.FirstName))
                .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.LastName))
                .ForMember(dest => dest.Email, opt => opt.MapFrom(src => src.Email))
                .ForMember(dest => dest.Password, opt => opt.MapFrom(src => src.Password))
                .ForMember(dest => dest.DateOfBirth, opt => opt.MapFrom(src => src.DateOfBirth))
                .ForMember(dest => dest.RoleId, opt => opt.MapFrom(src => src.RoleId))
                .ForMember(dest => dest.Role, opt => opt.MapFrom(src => src.Role));
        }
    }

}