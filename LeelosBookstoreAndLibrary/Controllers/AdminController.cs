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

        public ActionResult ViewAuthors()
        {
            try
            {
                var authors = db.Authors.ToList();
                var authorsModelList = new List<Models.Author>();
                foreach (var author in authors)
                {
                    var authorModel = MapperInstance.Mapper.Map<Models.Author>(author);
                    authorsModelList.Add(authorModel);
                }

                return View(authorsModelList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while retrieving users: " + ex.Message;
                return RedirectToAction("Error", "Account");
            }
        }

        public ActionResult AddAuthor()
        {
            return View();
        }

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

        public ActionResult AuthorDetails(int id)
        {
            Models.Author authorModel = new Models.Author();
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    DataLayer.Author author = db.Authors.FirstOrDefault(x => x.Id == id);
                    if (author != null)
                    {
                        authorModel = MapperInstance.Mapper.Map<Models.Author>(author);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Home");
                }
            }
            return View(authorModel);
        }

        public ActionResult EditAuthor(int id)
        {
            Models.Author authorModel = new Models.Author();
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    DataLayer.Author author = db.Authors.FirstOrDefault(x => x.Id == id);
                    if (author != null)
                    {
                        authorModel = MapperInstance.Mapper.Map<Models.Author>(author);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Account");
                }
            }
            return View(authorModel);
        }

        [HttpPost]
        public ActionResult EditAuthor(Models.Author authorModel)
        {
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    DataLayer.Author author = db.Authors.FirstOrDefault(x => x.Id == authorModel.Id);

                    if (author == null)
                    {
                        return HttpNotFound();
                    }

                    author.FirstName = authorModel.FirstName;
                    author.LastName = authorModel.LastName;
                    author.Biography = authorModel.Biography;
                    author.ImageMimeType = authorModel.ImageMimeType;
                    author.AuthorImage = authorModel.AuthorImage;

                    db.Entry(author).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    TempData["Message"] = "Changes saved successfully.";
                    return RedirectToAction("AuthorDetails", new { id = author.Id });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Account");
                }
            }
        }
        public ActionResult ViewPublishers()
        {
            try
            {
                var publishers = db.Publishers.ToList();
                var publishersModelList = new List<Models.Publisher>();
                foreach (var publisher in publishers)
                {
                    var publisherModel = MapperInstance.Mapper.Map<Models.Publisher>(publisher);
                    publishersModelList.Add(publisherModel);
                }

                return View(publishersModelList);
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = "An error occurred while retrieving users: " + ex.Message;
                return RedirectToAction("Error", "Account");
            }
        }
        public ActionResult AddPublisher()
        {
            return View();
        }

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

        public ActionResult PublisherDetails(int id)
        {
            Models.Publisher publisherModel = new Models.Publisher();
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    DataLayer.Publisher publisher = db.Publishers.FirstOrDefault(x => x.Id == id);
                    if (publisher != null)
                    {
                        publisherModel = MapperInstance.Mapper.Map<Models.Publisher>(publisher);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Home");
                }
            }
            return View(publisherModel);
        }

        public ActionResult EditPublisher(int id)
        {
            Models.Publisher publisherModel = new Models.Publisher();
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    DataLayer.Publisher publisher = db.Publishers.FirstOrDefault(x => x.Id == id);
                    if (publisher != null)
                    {
                        publisherModel = MapperInstance.Mapper.Map<Models.Publisher>(publisher);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Account");
                }
            }
            return View(publisherModel);
        }

        [HttpPost]
        public ActionResult EditPublisher(Models.Publisher publisherModel)
        {
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    DataLayer.Publisher publisher = db.Publishers.FirstOrDefault(x => x.Id == publisherModel.Id);

                    if (publisher == null)
                    {
                        return HttpNotFound();
                    }

                    publisher.Name = publisherModel.Name;
                    publisher.Email = publisherModel.Email;
                    publisher.Address = publisherModel.Address;
                    publisher.PhoneNumber = publisherModel.PhoneNumber;

                    db.Entry(publisher).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                    TempData["Message"] = "Changes saved successfully.";
                    return RedirectToAction("PublisherDetails", new { id = publisher.Id });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Account");
                }
            }
        }
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
