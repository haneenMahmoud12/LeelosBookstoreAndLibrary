using DataLayer;
using System;
using System.Collections.Generic;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeelosBookstoreAndLibrary.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index(String searchQuery)
        {
            List<Models.Book> booksList = new List<Models.Book>();
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            var books = db.Books.ToList();

            // If there's a search query, filter the books by title, author, or genre
            if (!String.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                books = books.Where(b => b.Title.ToLower().Contains(searchQuery) ||
                                         b.Genre.ToLower().Contains(searchQuery) ||
                                         b.Author.FirstName.ToLower().Contains(searchQuery) ||
                                         b.Author.LastName.ToLower().Contains(searchQuery)).ToList(); // Assuming you have a navigation property 'Author'
            }

            foreach (var book in books)
            {
                Models.Book bookModel = new Models.Book
                {
                    Id = book.Id,
                    Title = book.Title,
                    Description = book.Description,
                    StockQuantity = book.StockQuantity,
                    AuthorId = book.AuthorId,
                    Genre = book.Genre,
                    Price = (float)book.Price,
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
                };

                booksList.Add(bookModel);

            }
            return View(booksList);
        }

        public ActionResult AddBook()
        {
            Models.Book bookModel = new Models.Book();
            return View(bookModel);
        }

        [HttpPost]
        public ActionResult AddBook(Models.Book bookModel)
        {
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                Book book = new Book
                {
                    Title = bookModel.Title,
                    Description = bookModel.Description,
                    AuthorId = bookModel.AuthorId,
                    Genre = bookModel.Genre,
                    Price = bookModel.Price,
                    StockQuantity = bookModel.StockQuantity,
                    Rating = bookModel.Rating,
                    PublisherId = bookModel.PublisherId,
                    DatePublished = bookModel.DatePublished,
                    NumberOfPages = bookModel.NumberOfPages,
                    ImageData = bookModel.ImageData,
                    ImageMimeType = bookModel.ImageMimeType
                };

                db.Books.Add(book);
                db.SaveChanges();

                return RedirectToAction("Index");
            }
                //return View(bookModel);
        }

        public ActionResult EditBook(int id)
        {
            Models.Book bookModel = new Models.Book();
            using(LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                Book book = db.Books.FirstOrDefault(x => x.Id == id);
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
                        Price = (float)book.Price,
                        Rating = book.Rating,
                        PublisherId = book.PublisherId,
                        DatePublished = book.DatePublished,
                        NumberOfPages = book.NumberOfPages,
                        ImageData = book.ImageData,
                        ImageMimeType = book.ImageMimeType
                    };
                }
            }
            return View(bookModel);
        }

        [HttpPost]
        public ActionResult EditBook(Models.Book bookModel)
        {
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    // Fetch the book from the database
                    Book book = db.Books.FirstOrDefault(x => x.Id == bookModel.Id);

                    // Check if the book still exists
                    if (book == null)
                    {
                        return HttpNotFound(); // Return a 404 if the book is no longer in the database
                    }

                    // Update the book's properties
                    book.Title = bookModel.Title;
                    book.Description = bookModel.Description;
                    book.AuthorId = bookModel.AuthorId;
                    book.Genre = bookModel.Genre;
                    book.Price = bookModel.Price;
                    book.StockQuantity = bookModel.StockQuantity;
                    book.Rating = bookModel.Rating;
                    book.PublisherId = bookModel.PublisherId;
                    book.DatePublished = bookModel.DatePublished;
                    book.NumberOfPages = bookModel.NumberOfPages;
                    book.ImageData = bookModel.ImageData;
                    book.ImageMimeType = bookModel.ImageMimeType;

                    // Save the changes
                    db.Entry(book).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();

                    return RedirectToAction("Index");
                }
                catch (DbUpdateConcurrencyException)
                {
                    ModelState.AddModelError("", "The book you are trying to edit was modified or deleted by another user. Please reload and try again.");
                    return View(bookModel); // Return the view to allow the user to attempt the edit again
                }
               /* Book book = db.Books.FirstOrDefault(x => x.Id == bookModel.Id);
                if(book != null)
                {
                    book = new Book
                    {
                        Title = bookModel.Title,
                        Description = bookModel.Description,
                        AuthorId = bookModel.AuthorId,
                        Genre = bookModel.Genre,
                        Price = bookModel.Price,
                        StockQuantity = bookModel.StockQuantity,
                        Rating = bookModel.Rating,
                        PublisherId = bookModel.PublisherId,
                        DatePublished = bookModel.DatePublished,
                        NumberOfPages = bookModel.NumberOfPages,
                        ImageData = bookModel.ImageData,
                        ImageMimeType = bookModel.ImageMimeType
                    };

                    db.Entry(book).State = System.Data.Entity.EntityState.Modified;
                    db.SaveChanges();
                }

                return RedirectToAction("Index");*/
            }
            //return View(bookModel);
        }
        
        public ActionResult BookDetails(int id)
        {
            Models.Book bookModel = new Models.Book();
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                Book book = db.Books.FirstOrDefault(x => x.Id == id);
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
                        Price = (float)book.Price,
                        Rating = book.Rating,
                        PublisherId = book.PublisherId,
                        DatePublished = book.DatePublished,
                        NumberOfPages = book.NumberOfPages,
                        ImageData = book.ImageData,
                        ImageMimeType = book.ImageMimeType,
                        Author = new Models.Author
                        {
                            FirstName = book.Author.FirstName,
                            LastName = book.Author.LastName,
                        },
                        Publisher = new Models.Publisher
                        {
                            Name = book.Publisher.Name
                        }
                    };
                }
            }
            return View(bookModel);
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
