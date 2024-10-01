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

            var model = new Models.User
            {
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = (System.DateTime)user.DateOfBirth
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

            var model = new Models.User
            {
                Id = user.Id,
                FirstName = user.FirstName,
                LastName = user.LastName,
                Email = user.Email,
                DateOfBirth = (System.DateTime)user.DateOfBirth
            };

            return View(model);
        }


        // POST: Account/EditAccount
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult EditAccount(Models.User user, string currentPassword, string newPassword, string confirmNewPassword)
        {
            List<string> errorMessages = new List<string>();

            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                var userToUpdate = db.Users.FirstOrDefault(u => u.Id == user.Id);

                if (userToUpdate != null)
                {
                    userToUpdate.FirstName = user.FirstName;
                    userToUpdate.LastName = user.LastName;
                    userToUpdate.Email = user.Email;

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
