using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using DataLayer;
using LeelosBookstoreAndLibrary.Models;

namespace LeelosBookstoreAndLibrary.Controllers
{
    public class AdminController : Controller
    {
        private LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

        // Add Author GET
        public ActionResult AddAuthor()
        {
            return View();
        }

        // Add Author POST
        [HttpPost]
        public ActionResult AddAuthor(Models.Author author)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var authorEntity = MapperInstance.Mapper.Map<DataLayer.Author>(author);
                    db.Authors.Add(authorEntity);
                    db.SaveChanges();
                    TempData["Message"] = "Author added successfully.";
                    return RedirectToAction("AddAuthor");
                }
                TempData["ErrorMessage"] = "Failed to add author. Please check your inputs.";
                return View(author);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the author: " + ex.Message;
                return View(author);
            }
        }

        // Add Publisher GET
        public ActionResult AddPublisher()
        {
            return View();
        }

        // Add Publisher POST
        [HttpPost]
        public ActionResult AddPublisher(Models.Publisher publisher)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    var publisherEntity = MapperInstance.Mapper.Map<DataLayer.Publisher>(publisher);
                    db.Publishers.Add(publisherEntity);
                    db.SaveChanges();
                    TempData["Message"] = "Publisher added successfully.";
                    return RedirectToAction("AddPublisher");
                }
                TempData["ErrorMessage"] = "Failed to add publisher. Please check your inputs.";
                return View(publisher);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while adding the publisher: " + ex.Message;
                return View(publisher);
            }
        }

        // View Users GET
        public ActionResult ViewUsers()
        {
            try
            {
                var users = db.Users.Include("Role").ToList();
                var usersModelList = new List<Models.User>();
                foreach(var user in users)
                {
                    var userModel = MapperInstance.Mapper.Map<Models.User>(user);
                    usersModelList.Add(userModel);
                }
                
                return View(usersModelList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while retrieving users: " + ex.Message;
                return RedirectToAction("Error","Account");
            }
        }

        // Change User Role POST
        [HttpPost]
        public ActionResult ChangeUserRole(int userId, int newRoleId)
        {
            try
            {
                var user = db.Users.FirstOrDefault(u => u.Id == userId);
                if (user != null)
                {
                    user.RoleId = newRoleId;
                    db.Entry(user).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = $"User {user.FirstName} {user.LastName}'s role updated successfully.";
                }
                else
                {
                    TempData["ErrorMessage"] = "User not found.";
                }
                return RedirectToAction("ViewUsers");
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while changing the user's role: " + ex.Message;
                return RedirectToAction("ViewUsers");
            }
        }
    }
}
