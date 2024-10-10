using AutoMapper;
using DataLayer;
using LeelosBookstoreAndLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeelosBookstoreAndLibrary.Controllers
{
    public class HomeController : Controller
    {
        /*public ActionResult Index(string searchQuery, string sortOrder, int page = 1, int pageSize = 4)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

            try
            {
                // Filter by search query IF provided
                var booksQuery = db.Books.Include("Author").Include("Publisher").Where(b => b.isActive == true).AsQueryable();

                if (!string.IsNullOrEmpty(searchQuery))
                {
                    searchQuery = searchQuery.ToLower();
                    booksQuery = booksQuery.Where(b => b.Title.ToLower().Contains(searchQuery) ||
                                                       b.Genre.ToLower().Contains(searchQuery) ||
                                                       b.Author.FirstName.ToLower().Contains(searchQuery) ||
                                                       b.Author.LastName.ToLower().Contains(searchQuery));
                }

                switch (sortOrder)
                {
                    case "price":
                        booksQuery = booksQuery.OrderBy(b => b.Price);
                        break;
                    case "author":
                        booksQuery = booksQuery.OrderBy(b => b.Author.LastName);
                        break;
                    case "genre":
                        booksQuery = booksQuery.OrderBy(b => b.Genre);
                        break;
                    default: // Default sort by title
                        booksQuery = booksQuery.OrderBy(b => b.Title);
                        break;
                }
                var totalBooksCount = booksQuery.Count();

                var books = booksQuery
                                      .Skip((page - 1) * pageSize)
                                      .Take(pageSize)
                                      .Select(book => new Models.Book
                                      {
                                          Id = book.Id,
                                          Title = book.Title,
                                          Description = book.Description,
                                          StockQuantity = book.StockQuantity,
                                          AuthorId = book.AuthorId,
                                          Genre = book.Genre,
                                          Price = (float)Math.Round(book.Price, 2),
                                          Rating = book.Rating,
                                          PublisherId = book.PublisherId,
                                          DatePublished = book.DatePublished,
                                          NumberOfPages = book.NumberOfPages,
                                          ImageData = book.ImageData,
                                          ImageMimeType = book.ImageMimeType,
                                          Author = new Models.Author
                                          {
                                              FirstName = book.Author.FirstName,
                                              LastName = book.Author.LastName
                                          }
                                      }).ToList();

                var model = new Models.BooksViewModel
                {
                    Books = books,
                    CurrentPage = page,
                    PageSize = pageSize,
                    TotalBooksCount = totalBooksCount,
                    SearchQuery = searchQuery,
                    SortOrder = sortOrder
                };

                return View(model);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }*/


        public ActionResult Index()
        {
            try
            {
                using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
                {
                    // Base query: Active books with related Author and Publisher
                    var booksQuery = db.Books.Include("Author").Include("Publisher").Where(b => b.isActive == true);

                    // Check if the request is an AJAX request (from DataTables)
                    if (Request.IsAjaxRequest())
                    {
                        // Safely retrieve DataTables parameters
                        var draw = Request.Form.GetValues("draw")?.FirstOrDefault();
                        var start = Request.Form.GetValues("start")?.FirstOrDefault();
                        var length = Request.Form.GetValues("length")?.FirstOrDefault();
                        var sortColumnIndex = Request.Form.GetValues("order[0][column]")?.FirstOrDefault();
                        var sortColumn = Request.Form.GetValues($"columns[{sortColumnIndex}][data]")?.FirstOrDefault();
                        var sortColumnDir = Request.Form.GetValues("order[0][dir]")?.FirstOrDefault();
                        var searchValue = Request.Form.GetValues("search[value]")?.FirstOrDefault();

                        // Parse paging parameters with defaults
                        int pageSize = length != null ? Convert.ToInt32(length) : 4;
                        int skip = start != null ? Convert.ToInt32(start) : 0;

                        // Apply search filter if searchValue is provided
                        if (!string.IsNullOrEmpty(searchValue))
                        {
                            string lowerSearch = searchValue.ToLower();
                            booksQuery = booksQuery.Where(b =>
                                b.Title.ToLower().Contains(lowerSearch) ||
                                b.Genre.ToLower().Contains(lowerSearch) ||
                                b.Author.FirstName.ToLower().Contains(lowerSearch) ||
                                b.Author.LastName.ToLower().Contains(lowerSearch));
                        }

                        // Total records after filtering
                        var recordsFiltered = booksQuery.Count();

                        // Apply sorting based on the sortColumn and sortColumnDir
                        if (!string.IsNullOrEmpty(sortColumn) && !string.IsNullOrEmpty(sortColumnDir))
                        {
                            switch (sortColumn)
                            {
                                case "Title":
                                    booksQuery = sortColumnDir == "asc" ? booksQuery.OrderBy(b => b.Title) : booksQuery.OrderByDescending(b => b.Title);
                                    break;
                                case "Author":
                                    booksQuery = sortColumnDir == "asc" ? booksQuery.OrderBy(b => b.Author.LastName) : booksQuery.OrderByDescending(b => b.Author.LastName);
                                    break;
                                case "Genre":
                                    booksQuery = sortColumnDir == "asc" ? booksQuery.OrderBy(b => b.Genre) : booksQuery.OrderByDescending(b => b.Genre);
                                    break;
                                case "Price":
                                    booksQuery = sortColumnDir == "asc" ? booksQuery.OrderBy(b => b.Price) : booksQuery.OrderByDescending(b => b.Price);
                                    break;
                                default:
                                    booksQuery = sortColumnDir == "asc" ? booksQuery.OrderBy(b => b.Title) : booksQuery.OrderByDescending(b => b.Title);
                                    break;
                            }
                        }
                        else
                        {
                            // Default sorting if no sort parameters provided
                            booksQuery = booksQuery.OrderBy(b => b.Title);
                        }

                        // Apply paging
                        var books = booksQuery.Skip(skip).Take(pageSize).ToList();

                        // Prepare the response in the format expected by DataTables
                        var response = new
                        {
                            draw = draw,
                            recordsTotal = db.Books.Count(b => b.isActive == true),
                            recordsFiltered = recordsFiltered,
                            data = books.Select(b => new
                            {
                                b.Id,
                                b.Title,
                                Author = $"{b.Author.FirstName} {b.Author.LastName}",
                                b.Genre,
                                Price = b.Price.ToString("F2") // Format price as string with 2 decimal places
                            })
                        };

                        return Json(response, JsonRequestBehavior.AllowGet);
                    }
                    else
                    {
                        // Handle non-AJAX requests (initial page load)

                        // Optionally, you can pass an empty model or some initial data
                        var model = new BooksViewModel
                        {
                            Books = new List<Models.Book>(), // Empty list as DataTables will fetch data via AJAX
                            CurrentPage = 1,
                            PageSize = 4,
                            TotalBooksCount = db.Books.Count(b => b.isActive == true),
                            SearchQuery = string.Empty,
                            SortOrder = "Title"
                        };

                        return View(model);
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the exception (implementation depends on your logging framework)
                // For simplicity, redirect to an error page with the error message
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Error", "Account");
            }
        }



        public ActionResult Trial()
        {
            return View("HomePage");
        }

        public ActionResult GetBooks(string searchQuery, int page = 1, int pageSize = 4)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

            // Query for books
            var booksQuery = db.Books.Include("Author").Where(b => b.isActive == true).AsQueryable();

            // Apply search query if provided
            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                booksQuery = booksQuery.Where(b => b.Title.ToLower().Contains(searchQuery) ||
                                                   b.Genre.ToLower().Contains(searchQuery) ||
                                                   b.Author.FirstName.ToLower().Contains(searchQuery) ||
                                                   b.Author.LastName.ToLower().Contains(searchQuery));
            }

            var totalBooksCount = booksQuery.Count();

            var books = booksQuery
                              .OrderBy(b => b.Title) // Default ordering
                              .Skip((page - 1) * pageSize)
                              .Take(pageSize)
                              .Select(book => new Models.Book
                              {
                                  Id = book.Id,
                                  Title = book.Title,
                                  Author = new Models.Author
                                  {
                                      FirstName = book.Author.FirstName,
                                      LastName = book.Author.LastName
                                  },
                                  Genre = book.Genre,
                                  Price = (float)Math.Round(book.Price, 2)
                              }).ToList();

            return Json(new { data = books, recordsTotal = totalBooksCount, recordsFiltered = totalBooksCount }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult TestToastr()
        {
            return View();
        }
        public ActionResult AddBook()
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

            var authors = db.Authors.Select(a => new
            {
                Id = a.Id,
                Name = a.FirstName + " " + a.LastName
            }).ToList();
            var publishers = db.Publishers.ToList();


            ViewBag.Authors = new SelectList(authors, "Id", "Name");

            ViewBag.Publishers = new SelectList(publishers, "Id", "Name");
            Models.Book bookModel = new Models.Book();
            return View(bookModel);
        }

        [HttpPost]
        public ActionResult AddBook(Models.Book bookModel, HttpPostedFileBase ImageUpload)
        {
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImageUpload.InputStream))
                    {
                        bookModel.ImageData = binaryReader.ReadBytes(ImageUpload.ContentLength);
                        Console.WriteLine(bookModel.ImageData);
                    }

                    bookModel.ImageMimeType = ImageUpload.ContentType;
                }

                if (!ModelState.IsValid)
                {
                    ViewBag.Authors = new SelectList(db.Authors.ToList(), "Id", "Name");
                    ViewBag.Publishers = new SelectList(db.Publishers.ToList(), "Id", "Name");

                    return View(bookModel);
                }

                try
                {
                    DataLayer.Book book = new DataLayer.Book
                    {
                        Title = bookModel.Title,
                        Description = bookModel.Description,
                        AuthorId = bookModel.AuthorId,
                        Genre = bookModel.Genre,
                        Price = Math.Round(bookModel.Price, 2),
                        StockQuantity = bookModel.StockQuantity,
                        Rating = bookModel.Rating,
                        PublisherId = bookModel.PublisherId,
                        DatePublished = bookModel.DatePublished,
                        NumberOfPages = bookModel.NumberOfPages,
                        ImageData = bookModel.ImageData, 
                        ImageMimeType = bookModel.ImageMimeType, 
                        isActive = true
                    };

                    db.Books.Add(book);
                    db.SaveChanges();

                    TempData["Message"] = "Book added successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    ViewBag.Authors = new SelectList(db.Authors.ToList(), "Id", "Name");
                    ViewBag.Publishers = new SelectList(db.Publishers.ToList(), "Id", "Name");

                    TempData["ErrorMessage"] = "An error occurred: " + e.Message;
                    return View(bookModel);
                }
            }
        }


        public ActionResult EditBook(int id)
        {
            Models.Book bookModel = new Models.Book();
            using(LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    DataLayer.Book book = db.Books.FirstOrDefault(x => x.Id == id && x.isActive==true);
                    var authors = db.Authors.Select(a => new
                    {
                        Id = a.Id,
                        Name = a.FirstName + " " + a.LastName
                    }).ToList();
                    var publishers = db.Publishers.ToList();


                    ViewBag.Authors = new SelectList(authors, "Id", "Name");

                    ViewBag.Publishers = new SelectList(publishers, "Id", "Name");
                    if (book != null)
                    {
                        bookModel = new Models.Book
                        {
                            Id = book.Id,
                            Title = book.Title,
                            Description = book.Description,
                            StockQuantity = book.StockQuantity,
                            AuthorId = book.AuthorId,
                            Genre = book.Genre,
                            Price = (float)Math.Round(book.Price, 2),
                            Rating = book.Rating,
                            PublisherId = book.PublisherId,
                            DatePublished = book.DatePublished,
                            NumberOfPages = book.NumberOfPages,
                            ImageData = book.ImageData,
                            ImageMimeType = book.ImageMimeType
                        };
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Account");
                }
            }
            return View(bookModel);
        }

        [HttpPost]
        public ActionResult EditBook(Models.Book bookModel, HttpPostedFileBase ImageUpload)
        {
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                if (ImageUpload != null && ImageUpload.ContentLength > 0)
                {
                    using (var binaryReader = new BinaryReader(ImageUpload.InputStream))
                    {
                        bookModel.ImageData = binaryReader.ReadBytes(ImageUpload.ContentLength);
                    }

                    bookModel.ImageMimeType = ImageUpload.ContentType;
                }
                try
                {
                    DataLayer.Book book = db.Books.FirstOrDefault(x => x.Id == bookModel.Id && x.isActive==true);

                    if (book == null)
                    {
                        return HttpNotFound();
                    }

                    book.Title = bookModel.Title;
                    book.Description = bookModel.Description;
                    book.AuthorId = bookModel.AuthorId;
                    book.Genre = bookModel.Genre;
                    book.Price = Math.Round(bookModel.Price,2);
                    book.StockQuantity = bookModel.StockQuantity;
                    book.Rating = bookModel.Rating;
                    book.PublisherId = bookModel.PublisherId;
                    book.DatePublished = bookModel.DatePublished;
                    book.NumberOfPages = bookModel.NumberOfPages;
                    book.ImageData = bookModel.ImageData;
                    book.ImageMimeType = bookModel.ImageMimeType;

                    db.Entry(book).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    TempData["Message"] = "Changes saved successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Home");
                }
            }
        }
        
        public ActionResult BookDetails(int id)
        {
            Models.Book bookModel = new Models.Book();
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    DataLayer.Book book = db.Books.FirstOrDefault(x => x.Id == id && x.isActive == true);
                    if (book != null)
                    {
                        bookModel = MapperInstance.Mapper.Map<DataLayer.Book, Models.Book>(book);
                    }
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Home");
                }
            }
            return View(bookModel);
        }

        [HttpPost]
       // [ValidateAntiForgeryToken]
        public ActionResult DeleteBook(int id)
        {
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    DataLayer.Book book = db.Books.Find(id);

                    if (book == null)
                    {
                        return HttpNotFound();
                    }
                    else if(book.isActive == false)
                    {
                        ModelState.AddModelError("", "The book you are trying to edit was modified or deleted by another user. Please reload and try again.");
                        TempData["ErrorMessage"] = "The book you are trying to edit was modified or deleted by another user. Please reload and try again.";
                        return RedirectToAction("Index");
                    }
                    else
                    {
                        book.isActive = false;
                        db.Entry(book).State = System.Data.Entity.EntityState.Modified;
                        db.SaveChanges();
                    }

                    TempData["Message"] = "Book deleted successfully.";
                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "The book you are trying to edit was modified or deleted by another user. Please reload and try again.");
                    TempData["ErrorMessage"] = "The book you are trying to edit was modified or deleted by another user. Please reload and try again.";
                    return RedirectToAction("Index");
                }
            }
        }

        public ActionResult About()
            {
                ViewBag.Message = "Your application description page.";

                return View();
            }

        public ActionResult Contact()
            {
                ViewBag.Message = "Your contact page.";

                return View();
            }
    }
}
