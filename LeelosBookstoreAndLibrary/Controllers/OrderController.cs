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
            if (Session["Process"].Equals("Buy"))
                return BuyCheckout();
            else
                return BorrowCheckout();
        }

        public ActionResult BuyCheckout()
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
                    Price = Math.Round((decimal)(item.ShoppingCartItem.Price * item.ShoppingCartItem.Quantity),2),
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
                    TotalPrice = Math.Round(totalPrice,2),
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

        public ActionResult BorrowCheckout()
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

            var userId = Session["UserId"] as int?;
            if (userId.HasValue)
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                var address = db.Addresses.FirstOrDefault(a => a.UserId == userId);
                var cart = db.BorrowCarts.FirstOrDefault(c => c.UserId == userId);
                var cartItems = db.BorrowCartItems.Where(ci => ci.BorrowCartId == cart.Id).Select(ci => new Models.BorrowCartItem
                {
                    Id = ci.Id,
                    BorrowCartId = ci.Id,
                    BookId = ci.BookId,
                    BorrowCart = new Models.BorrowCart
                    {
                        Id = cart.Id,
                        UserId = cart.UserId
                    },
                    Book = new Models.Book
                    {
                        Id = ci.BookId,
                        Title = ci.Book.Title,
                        Price = (float)Math.Round(ci.Book.Price,2)
                    }
                }).ToList();

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

                decimal totalPrice = 0;
                foreach (var item in cartItems)
                {
                    decimal borrowPrice = (decimal)(item.Book.Price * 0.25);
                    totalPrice += borrowPrice;
                }

                var model = new BorrowCheckoutViewModel
                {
                    User = userModel,
                    ShippingInfo = shippingInfo,
                    cartItems = cartItems,
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
            if (Session["Process"].Equals("Buy"))
                return BuyOrder(price);
            else
                return BorrowOrder(price);
        }

        public ActionResult BorrowOrder(decimal price)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            var userId = Session["UserId"] as int?;
            if (userId.HasValue)
            {
                DataLayer.Order newOrder = new DataLayer.Order
                {
                    UserId = (int)userId,
                    TotalPrice = Math.Round(price,2),
                    OrderDate = DateTime.Now,
                    Status = "Confirmed"
                };
                db.Orders.Add(newOrder);
                db.SaveChanges(); // Save to get the cart's ID

                var cart = db.BorrowCarts.FirstOrDefault(c => c.UserId == userId);
                var cartItems = db.BorrowCartItems
                    .Where(cs => cs.BorrowCartId == cart.Id)
                   /* .Select(cs => new DataLayer.BorrowCartItem
                    {
                        Id = cs.Id,
                        BorrowCartId = cs.BorrowCartId,
                        BookId = cs.BookId
                    })*/
                    .ToList();

                foreach (var item in cartItems)
                {
                    DataLayer.OrderItem orderItem = new DataLayer.OrderItem
                    {
                        OrderId = newOrder.Id,
                        BookId = item.Book.Id,
                        Price = Math.Round((decimal)(item.Book.Price * 0.25), 2),
                        Quantity = 1
                    };
                    db.OrderItems.Add(orderItem);
                    

                    var borrow = new DataLayer.Borrow
                    {
                        UserId = userId.Value,
                        BookId = item.BookId,
                        BorrowDate = DateTime.Now,
                        DueDate = DateTime.Now.AddDays(14), // 14-day borrow period
                        IsReturned = false,
                        BorrowFee = (decimal)(item.Book.Price * 0.25),
                        LateFee = 0
                    };

                    db.Borrows.Add(borrow);

                    // Reduce stock quantity
                    var book = db.Books.FirstOrDefault(b => b.Id == item.BookId);
                    book.StockQuantity -= 1;

                    db.BorrowCartItems.Remove(item);

                    db.SaveChanges();
                }

                return RedirectToAction("OrderSuccess");
            }
            else
            {
                return RedirectToAction("Checkout", "Order");
            }
        }

        public ActionResult BuyOrder(decimal price)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            var userId = Session["UserId"] as int?;
            if (userId.HasValue)
            {
                DataLayer.Order newOrder = new DataLayer.Order
                {
                    UserId = (int)userId,
                    TotalPrice = Math.Round(price,2),
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
                        Price = Math.Round((decimal)item.ShoppingCartItem.Price,2),
                        Quantity = item.ShoppingCartItem.Quantity
                    };
                    db.OrderItems.Add(orderItem);
                    db.ShoppingCartItems.Remove(item.ShoppingCartItem);

                    var book = db.Books.FirstOrDefault(b => b.Id == item.Book.Id);
                    book.StockQuantity -= 1;

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