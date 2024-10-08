// Controllers/AuthController.cs
using DataLayer;
using LeelosBookstoreAndLibrary.Models;
using System;
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
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    if (ModelState.IsValid)
                    {
                        if (db.Users.Any(u => u.Email == user.Email))
                        {
                            ModelState.AddModelError("Email", "This email is already registered.");
                            return View(user);
                        }

                        var userToAdd = MapperInstance.Mapper.Map<Models.User, DataLayer.User>(user);
                        db.Users.Add(userToAdd);
                        db.SaveChanges();

                        Session["UserId"] = userToAdd.Id;
                        Session["UserRole"] = userToAdd.RoleId;
                        Session["UserName"] = $"{userToAdd.FirstName} {userToAdd.LastName}";

                        return RedirectToAction("Login");
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Home");
                }
            }

            return View(user); // Ensure this returns the user object with validation messages
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
            try
            {
                var existingUser = db.Users.FirstOrDefault(u => u.Email == user.Email);

                if (existingUser == null)
                {
                    // Email not found
                    ModelState.AddModelError("Email", "No such email found.");
                }
                else if (existingUser.Password != user.Password)
                {
                    // Incorrect password
                    ModelState.AddModelError("Password", "Wrong password.");
                }
                else
                {
                    // Successful login
                    Session["UserId"] = existingUser.Id;
                    Session["UserRole"] = existingUser.RoleId;
                    Session["UserName"] = $"{existingUser.FirstName} {existingUser.LastName}";
                    return RedirectToAction("Index", "Home");
                }
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
            return View(user);
        }

        // GET: Account/Logout
        public ActionResult Logout()
        {
            Session.Abandon(); // Clear the session
            return RedirectToAction("Index", "Home");
        }

        // GET: Account/ViewAccount

        public ActionResult ViewAccount(int orderPage = 1, int orderPageSize = 2, int borrowPage = 1, int borrowPageSize = 4)
        {
            try
            {
                using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
                {
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

                    var userModel = UserInfo(user);

                    var address = db.Addresses.FirstOrDefault(a => a.UserId == user.Id);
                    ShippingInfo addressModel = UserAddress(address);

                    // Paging for Orders
                    var totalOrdersCount = db.Orders.Count(order => order.UserId == userId);
                    var ordersQuery = UserOrders(totalOrdersCount, orderPage, orderPageSize, (int)userId);

                    // Paging for Borrowed Books
                    var totalBorrowedBooksCount = db.Borrows.Count(bb => bb.UserId == user.Id);
                    var borrowedBooksQuery = UserBorrows(totalBorrowedBooksCount, borrowPage, borrowPageSize, (int)userId);

                    var model = new UserViewModel
                    {
                        Id = user.Id,
                        FirstName = user.FirstName,
                        LastName = user.LastName,
                        DateOfBirth = (DateTime)user.DateOfBirth,
                        Email = user.Email,
                        Address = addressModel,
                        Orders = ordersQuery,
                        BorrowedBooks = borrowedBooksQuery,
                        OrderPage = orderPage,
                        OrderPageSize = orderPageSize,
                        BorrowPage = borrowPage,
                        BorrowPageSize = borrowPageSize,
                        TotalOrders = totalOrdersCount,  // Total count without paging
                        TotalBorrowedBooks = totalBorrowedBooksCount  // Total count without paging
                    };

                    return View(model);
                }
            }
            catch (Exception ex)
            {
                //  NLog or log4net
                // Logger.Error(ex, "Error in ViewAccount method.");

                TempData["ErrorMessage"] = "An error occurred while retrieving your account information. Please try again later.";

                return RedirectToAction("Error", "Home");
            }
        }

        public Models.User UserInfo(DataLayer.User user)
        {
            var userModel = new Models.User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = (System.DateTime)user.DateOfBirth
            };
            return userModel;
        }

        public Models.ShippingInfo UserAddress(DataLayer.Address address)
        {
            ShippingInfo addressModel = new ShippingInfo
            {
                Address = address.Address1,
                City = address.City,
                Governorate = address.Governorate,
                Country = address.Country,
                ZipCode = address.ZipCode,
                PhoneNumber = address.PhoneNumber
            };
            return addressModel;
        }

        public List<Models.Order> UserOrders(int totalOrdersCount, int orderPage, int orderPageSize, int userId)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

            // Paging for Orders
            var ordersQuery = db.Orders.Where(order => order.UserId == userId)
                                        .OrderBy(o => o.OrderDate)
                                        .Skip((orderPage - 1) * orderPageSize)
                                        .Take(orderPageSize)
                                        .Select(o => new Models.Order
                                        {
                                            Id = o.Id,
                                            UserId = o.UserId,
                                            OrderDate = o.OrderDate,
                                            Status = o.Status,
                                            TotalPrice = o.TotalPrice,
                                            OrderItems = db.OrderItems.Where(oi => oi.OrderId == o.Id)
                                                .Select(oi => new Models.OrderItem
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
            return ordersQuery;
        }

        public List<Models.Borrow> UserBorrows(int totalBorrowedBooksCount, int borrowPage, int borrowPageSize, int userId)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            var borrowedBooksQuery = db.Borrows.Where(bb => bb.UserId == userId)
                                               .OrderBy(bb => bb.BorrowDate)
                                               .Skip((borrowPage - 1) * borrowPageSize)
                                               .Take(borrowPageSize)
                                               .Select(bb => new Models.Borrow
                                               {
                                                   Id = bb.Id,
                                                   UserId = userId,
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
            return borrowedBooksQuery;
        }

        // GET: Account/EditAccount
        public ActionResult EditAccount()
        {
            try
            {
                var userId = Session["UserId"] as int?;

                if (!userId.HasValue)
                {
                    return RedirectToAction("Login");
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
            catch(Exception e)
            {
                TempData["ErrorMessage"] = e.Message;

                return RedirectToAction("Error", "Home"); 
            }
        }

        // POST: Account/EditAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(Models.UserViewModel user)
        {
            try
            {
                List<string> errorMessages = new List<string>();

                using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
                {
                    var userToUpdate = db.Users.FirstOrDefault(u => u.Id == user.Id);
                    var userAddress = db.Addresses.FirstOrDefault(a => a.UserId == user.Id);

                    if (userToUpdate == null) return View(user); // User not found

                    // Update user details
                    userToUpdate.FirstName = user.FirstName;
                    userToUpdate.LastName = user.LastName;
                    userToUpdate.Email = user.Email;

                    // Update address if it exists
                    if (userAddress != null)
                    {
                        userAddress.PhoneNumber = user.Address.PhoneNumber;
                        userAddress.Address1 = user.Address.Address;
                        userAddress.City = user.Address.City;
                        userAddress.Governorate = user.Address.Governorate;
                        userAddress.Country = user.Address.Country;
                        userAddress.ZipCode = user.Address.ZipCode;
                    }

                    

                    if (errorMessages.Count > 0)
                    {
                        ViewBag.ErrorMessages = errorMessages;
                        return View(user); // Return with errors
                    }

                    // Update the user and address if all validations passed
                    db.Entry(userToUpdate).State = System.Data.Entity.EntityState.Modified;
                    if (userAddress != null)
                    {
                        db.Entry(userAddress).State = System.Data.Entity.EntityState.Modified;
                    }
                    db.SaveChanges();

                    // Update session with new username
                    Session["UserName"] = $"{userToUpdate.FirstName} {userToUpdate.LastName}";

                    return RedirectToAction("ViewAccount");
                }
            }
            catch (Exception ex)
            {
                // Log the exception (consider using a logging framework)
                ViewBag.ErrorMessages = new List<string> { "An error occurred while updating your account. Please try again later." };
                return View(user); // Return the view with an error message
            }
        }

        public ActionResult ChangePassword()
        {
            return View(new ChangePasswordViewModel());
        }

        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordViewModel model)
        {
            try
            {
                int userId = (int)Session["UserId"];
                using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
                {
                    var userToUpdate = db.Users.FirstOrDefault(u => u.Id == userId);
                    if (!string.IsNullOrEmpty(model.CurrentPassword))
                    {
                        if (!userToUpdate.Password.Equals(model.CurrentPassword))
                        {
                            model.ErrorMessages.Add("Current password is incorrect.");
                        }

                        if (!string.IsNullOrEmpty(model.NewPassword))
                        {
                            if (model.NewPassword.Length < 6)
                            {
                                model.ErrorMessages.Add("Password must be at least 6 characters long.");
                            }
                            else if (!model.NewPassword.Equals(model.ConfirmNewPassword))
                            {
                                model.ErrorMessages.Add("New password and confirmation do not match.");
                            }
                            else
                            {
                                userToUpdate.Password = model.NewPassword;
                            }
                        }
                    }

                    if (model.ErrorMessages.Count > 0)
                    {
                        return View(model); // Return with errors
                    }

                    // Update the user if all validations passed
                    db.Entry(userToUpdate).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Password changed successfully.";
                    return RedirectToAction("Index", "Home"); // Redirect to the desired page
                }
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while changing your password. Please try again later.";
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult Error()
        {
            return View();
        }

    }
}
