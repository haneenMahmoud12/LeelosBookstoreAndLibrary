using AutoMapper;
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
        public ActionResult Index(string searchQuery, int page = 1, int pageSize = 4)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

            // Filter by search query IF provided
            var booksQuery = db.Books.Include("Author").Include("Publisher").AsQueryable();

            if (!string.IsNullOrEmpty(searchQuery))
            {
                searchQuery = searchQuery.ToLower();
                booksQuery = booksQuery.Where(b => b.Title.ToLower().Contains(searchQuery) ||
                                                   b.Genre.ToLower().Contains(searchQuery) ||
                                                   b.Author.FirstName.ToLower().Contains(searchQuery) ||
                                                   b.Author.LastName.ToLower().Contains(searchQuery));
            }

            var totalBooksCount = booksQuery.Count();

            var books = booksQuery.OrderBy(b => b.Title)
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
                SearchQuery = searchQuery
            };

            return View(model);
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
                    Price = Math.Round(bookModel.Price,2),
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
                        Price = (float) Math.Round(book.Price,2),
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
                    Book book = db.Books.FirstOrDefault(x => x.Id == bookModel.Id);

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

                    return RedirectToAction("Index");
                }
                catch (Exception)
                {
                    ModelState.AddModelError("", "The book you are trying to edit was modified or deleted by another user. Please reload and try again.");
                    return View(bookModel);
                }
            }
        }
        
        public ActionResult BookDetails(int id)
        {
            Models.Book bookModel = new Models.Book();
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                Book book = db.Books.FirstOrDefault(x => x.Id == id);
                if (book != null)
                {
                    /*bookModel = new Models.Book
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
                    };*/
                    bookModel = MapperInstance.Mapper.Map<DataLayer.Book, Models.Book>(book);
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
