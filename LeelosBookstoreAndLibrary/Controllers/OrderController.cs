using DataLayer;
using LeelosBookstoreAndLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeelosBookstoreAndLibrary.Controllers
{
    public class OrderController : Controller
    {
        // GET: Order
        public ActionResult Checkout()
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

            var userId = Session["UserId"] as int?;
            if (userId.HasValue)
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                var address = db.Addresses.FirstOrDefault(a => a.UserId == userId);
                var cart = db.ShoppingCarts.FirstOrDefault(c => c.UserId == userId);
                var cartItems = db.ShoppingCart_ShoppingCartItems
                    .Where(cs => cs.ShoppingCartId == cart.Id)
                    .Select(cs => new
                    {
                        ShoppingCartItem = cs.ShoppingCartItem,
                        Book = cs.ShoppingCartItem.Book
                    })
                    .ToList();

                var userModel = new Models.User
                {
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Email = user.Email
                };

                var shippingInfo = new ShippingInfo
                {
                    Address = address.Address1,
                    City = address.City,
                    Governorate = address.Governorate,
                    ZipCode = address.ZipCode,
                    Country = address.Country,
                    PhoneNumber = address.PhoneNumber
                };

                var shoppingCartItemsModel = cartItems.Select(item => new LeelosBookstoreAndLibrary.Models.ShoppingCartItem
                {
                    Id = item.ShoppingCartItem.Id,
                    BookId = item.ShoppingCartItem.BookId,
                    Quantity = item.ShoppingCartItem.Quantity,
                    Price = (decimal)item.ShoppingCartItem.Price * item.ShoppingCartItem.Quantity,
                    Book = new LeelosBookstoreAndLibrary.Models.Book
                    {
                        Title = item.Book.Title,
                        StockQuantity = item.Book.StockQuantity
                    }
                }).ToList();

                decimal totalPrice = 0;
                foreach (var item in shoppingCartItemsModel)
                {
                    totalPrice += item.Price;
                }

                var model = new CheckoutViewModel
                {
                    User = userModel,
                    ShippingInfo = shippingInfo,
                    cartItems = shoppingCartItemsModel,
                    TotalPrice = totalPrice,
                    AddressId = address.AddressId,
                    PaymentMethod = "Cash on Delivery"
                };

                return View(model);
            }
            else
            {
                return RedirectToAction("Login");
            }
            
        }

        [HttpPost]
        public ActionResult ConfirmOrder(decimal price)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            var userId = Session["UserId"] as int?;
            if (userId.HasValue)
            {
                DataLayer.Order newOrder = new DataLayer.Order
                {
                    UserId = (int)userId,
                    TotalPrice = price,
                    OrderDate = DateTime.Now,
                    Status = "Confirmed"
                };
                db.Orders.Add(newOrder);
                db.SaveChanges(); // Save to get the cart's ID

                var cart = db.ShoppingCarts.FirstOrDefault(c => c.UserId == userId);
                var cartItems = db.ShoppingCart_ShoppingCartItems
                    .Where(cs => cs.ShoppingCartId == cart.Id)
                    .Select(cs => new
                    {
                        ShoppingCartItem = cs.ShoppingCartItem,
                        Book = cs.ShoppingCartItem.Book
                    })
                    .ToList();

                foreach (var item in cartItems)
                {
                    DataLayer.OrderItem orderItem = new DataLayer.OrderItem
                    {
                        OrderId = newOrder.Id,
                        BookId = item.Book.Id,
                        Price = (decimal)item.ShoppingCartItem.Price,
                        Quantity = item.ShoppingCartItem.Quantity
                    };
                    db.OrderItems.Add(orderItem);
                    db.ShoppingCartItems.Remove(item.ShoppingCartItem);
                    db.SaveChanges();
                }

                return RedirectToAction("OrderSuccess");
            }
            else
            {
                return RedirectToAction("Checkout", "Order");
            }
        }

        public ActionResult OrderSuccess()
        {
            return View("OrderSuccess");
        }

        [HttpGet]
        public ActionResult CancelOrder(int id)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            var order = db.Orders.Find(id);
            if (order != null)
            {
                var orderItems = db.OrderItems.Where(oi => oi.OrderId == order.Id).ToList();

                foreach(var item in orderItems)
                {
                    db.OrderItems.Remove(item);
                }

                // Now remove the ShoppingCartItem
                db.Orders.Remove(order);
                db.SaveChanges();
            }

            return RedirectToAction("ViewAccount","Account");
        }
    }
}