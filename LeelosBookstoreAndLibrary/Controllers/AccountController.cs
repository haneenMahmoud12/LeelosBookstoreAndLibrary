// Controllers/AuthController.cs
using DataLayer;
using LeelosBookstoreAndLibrary.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;

namespace LeelosBookstoreAndLibrary.Controllers
{
    public class AccountController : Controller
    {

        // GET: Account/Register
        public ActionResult Register()
        {
            return View();
        }

        // POST: Account/Register
        [HttpPost]
        public ActionResult Register(Models.User user)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            if (ModelState.IsValid)
            {
                if (db.Users.Any(u => u.Email == user.Email))
                {
                    ModelState.AddModelError("Email", "This email is already registered.");
                    return View(user);
                }

                DataLayer.User userToAdd = new DataLayer.User
                {
                    Email = user.Email,
                    FirstName = user.FirstName,
                    LastName = user.LastName,
                    Password = user.Password,
                    DateOfBirth = user.DateOfBirth,
                    RoleId = 1
                };
                db.Users.Add(userToAdd);
                db.SaveChanges();

                Session["UserId"] = userToAdd.Id;
                Session["UserRole"] = userToAdd.RoleId;
                Session["UserName"] = $"{userToAdd.FirstName} {userToAdd.LastName}";

                return RedirectToAction("Login");
            }
            return View(user);
        }

        // GET: Account/Login
        public ActionResult Login()
        {
            return View();
        }

        // POST: Account/Login
        [HttpPost]
        public ActionResult Login(Models.User user)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email && u.Password == user.Password);
            if (existingUser != null)
            {
                Session["UserId"] = existingUser.Id;
                Session["UserRole"] = existingUser.RoleId;
                Session["UserName"] = $"{existingUser.FirstName} {existingUser.LastName}";
                return RedirectToAction("Index", "Home");
            }
            ModelState.AddModelError("", "Invalid login attempt.");
            return View(user);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Abandon(); // Clear the session
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/ViewAccount
        public ActionResult ViewAccount()
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            var userId = Session["UserId"] as int?;

            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var user = db.Users.FirstOrDefault(u => u.Id == userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            var userModel = new Models.User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = (System.DateTime)user.DateOfBirth
            };

            var address = db.Addresses.FirstOrDefault(a => a.UserId == user.Id);
            ShippingInfo addressModel = new ShippingInfo
            {
                Address = address.Address1,
                City = address.City,
                Governorate = address.Governorate,
                Country = address.Country,
                ZipCode = address.ZipCode,
                PhoneNumber = address.PhoneNumber
            };

            var orders = db.Orders.Where(order => order.UserId == userId).Select(o => new Models.Order {
                Id = o.Id,
                UserId = o.UserId,
                OrderDate = o.OrderDate,
                Status = o.Status,
                TotalPrice = o.TotalPrice,
                OrderItems = db.OrderItems.Where(oi => oi.OrderId == o.Id).Select(oi => new Models.OrderItem
                {
                    Book = new Models.Book
                    {
                        Id = oi.Book.Id,
                        Title = oi.Book.Title,
                        AuthorId = oi.Book.AuthorId,
                        PublisherId = oi.Book.PublisherId,
                        Price = (float)oi.Book.Price,
                        StockQuantity = oi.Book.StockQuantity,
                        Genre = oi.Book.Genre,
                        DatePublished = oi.Book.DatePublished,
                        Description = oi.Book.Description,
                        Rating = oi.Book.Rating,
                        NumberOfPages = oi.Book.NumberOfPages,
                        ImageData = oi.Book.ImageData,
                        ImageMimeType = oi.Book.ImageMimeType
                    },
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList()
        }).ToList();

            foreach(var order in orders)
            {
                var orderItem = db.OrderItems.Where(oi => oi.OrderId == order.Id).Select(oi => new Models.OrderItem
                {
                    Book = new Models.Book
                    {
                        Id = oi.Book.Id,
                        Title = oi.Book.Title,
                        AuthorId = oi.Book.AuthorId,
                        PublisherId = oi.Book.PublisherId,
                        Price = (float)oi.Book.Price,
                        StockQuantity = oi.Book.StockQuantity,
                        Genre = oi.Book.Genre,
                        DatePublished = oi.Book.DatePublished,
                        Description = oi.Book.Description,
                        Rating = oi.Book.Rating,
                        NumberOfPages = oi.Book.NumberOfPages,
                        ImageData = oi.Book.ImageData,
                        ImageMimeType = oi.Book.ImageMimeType
                    },
                    Quantity = oi.Quantity,
                    Price = oi.Price
                }).ToList();
            }

            var borrowedBooks = db.Borrows.Where(bb => bb.UserId == user.Id).Select(bb => new Models.Borrow
            {
                Id = bb.Id,
                UserId = user.Id,
                BookId = bb.BookId,
                BorrowDate = bb.BorrowDate,
                DueDate = bb.DueDate,
                ReturnDate = bb.ReturnDate,
                IsReturned = bb.IsReturned,
                borrowFee = bb.BorrowFee ?? 0,
                LateFee = bb.LateFee ?? 0,
                Book = new Models.Book
                {
                    Title = bb.Book.Title
                }
            }).ToList();

            var model = new UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                DateOfBirth = (System.DateTime)user.DateOfBirth,
                Email = user.Email,
                Address = addressModel,
                Orders = orders,
                BorrowedBooks = borrowedBooks
            };

            return View(model);
        }

        // GET: Account/EditAccount
        public ActionResult EditAccount()
        {
            var userId = Session["UserId"] as int?;

            if (!userId.HasValue)
            {
                return RedirectToAction("Login"); // Redirect if not logged in
            }

            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            var user = db.Users.Find(userId);

            if (user == null)
            {
                return HttpNotFound();
            }

            var address = db.Addresses.FirstOrDefault(a => a.UserId == user.Id);

            var model = new Models.UserViewModel
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = (System.DateTime)user.DateOfBirth,
                Address = new ShippingInfo
                {
                    Address = address.Address1,
                    City = address.City,
                    Country = address.Country,
                    Governorate = address.Governorate,
                    ZipCode = address.ZipCode,
                    PhoneNumber = address.PhoneNumber
                }
            };

            

            return View(model);
        }


        // POST: Account/EditAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(Models.UserViewModel user, string currentPassword, string newPassword, string confirmNewPassword)
        {
            List<string> errorMessages = new List<string>();

            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                var userToUpdate = db.Users.FirstOrDefault(u => u.Id == user.Id);
                var userAddress = db.Addresses.FirstOrDefault(a => a.UserId == user.Id);

                if (userToUpdate != null)
                {
                    userToUpdate.FirstName = user.FirstName;
                    userToUpdate.LastName = user.LastName;
                    userToUpdate.Email = user.Email;

                    if(userAddress != null)
                    {
                        userAddress.PhoneNumber = user.Address.PhoneNumber;
                        userAddress.Address1 = user.Address.Address;
                        userAddress.City = user.Address.City;
                        userAddress.Governorate = user.Address.Governorate;
                        userAddress.Country = user.Address.Country;
                        userAddress.ZipCode = user.Address.ZipCode;
                    }

                    if (!string.IsNullOrEmpty(currentPassword))
                    {
                        if (!userToUpdate.Password.Equals(currentPassword))
                        {
                            errorMessages.Add("Current password is incorrect.");
                        }

                        if (!string.IsNullOrEmpty(newPassword))
                        {
                            if (newPassword.Length < 6)
                            {
                                errorMessages.Add("Password must be at least 6 characters long.");
                            }
                            else if (!newPassword.Equals(confirmNewPassword))
                            {
                                errorMessages.Add("New password and confirmation do not match.");
                            }
                            else
                            {
                                userToUpdate.Password = newPassword;
                            }
                        }
                    }

                    if (errorMessages.Count > 0)
                    {
                        ViewBag.ErrorMessages = errorMessages;
                        return View(user);
                    }

                    db.SaveChanges();
                    Session["UserName"] = $"{userToUpdate.FirstName} {userToUpdate.LastName}";
                    return RedirectToAction("ViewAccount");
                }
            }
            return View(user);
        }

    }
}
