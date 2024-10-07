using DataLayer;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeelosBookstoreAndLibrary.Controllers
{
    public class BorrowController : Controller
    {
        private LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

        [HttpPost]
        public ActionResult AddToBorrowCart(int bookId)
        {
            var userId = Session["UserId"] as int?;
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            // Find or create the borrow cart for the user
            var borrowCart = db.BorrowCarts.FirstOrDefault(c => c.UserId == userId.Value);
            if (borrowCart == null)
            {
                borrowCart = new BorrowCart { UserId = userId.Value };
                db.BorrowCarts.Add(borrowCart);
                db.SaveChanges();
            }

            // Check if the book is already in the borrow cart
            var existingItem = db.BorrowCartItems.FirstOrDefault(i => i.BorrowCartId == borrowCart.Id && i.BookId == bookId);
            if (existingItem == null)
            {
                var newItem = new BorrowCartItem { BorrowCartId = borrowCart.Id, BookId = bookId };
                db.BorrowCartItems.Add(newItem);
                db.SaveChanges();
            }

            return RedirectToAction("ViewBorrowCart"); 
        }

        public ActionResult ViewBorrowCart()
        {
            var userId = Session["UserId"] as int?;
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var borrowCart = db.BorrowCarts.FirstOrDefault(c => c.UserId == userId.Value);
            if (borrowCart != null)
            {
                var borrowCartItems = db.BorrowCartItems.Where(i => i.BorrowCartId == borrowCart.Id).Select(b => new Models.BorrowCartItem
                {
                    Id = b.Id,
                    BookId = b.BookId,
                    BorrowCartId = b.BorrowCartId,
                    Book = new Models.Book
                    {
                        Title = b.Book.Title,
                        Price = (float)Math.Round(b.Book.Price,2)
                    }
                }).ToList();
                return View(borrowCartItems);
            }
            else
            {
                var newBorrowCart = new BorrowCart
                {
                    UserId = userId.Value
                };

                db.BorrowCarts.Add(newBorrowCart);
                db.SaveChanges();

                return View(new List<BorrowCartItem>());
            }
        }

        public ActionResult RemoveFromBorrowCart(int id)
        {
            var item = db.BorrowCartItems.FirstOrDefault(i => i.Id == id);
            if (item != null)
            {
                db.BorrowCartItems.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("ViewBorrowCart");
        }


        [HttpPost]
        public ActionResult BorrowBook(int bookId)
        {
            var userId = Session["UserId"] as int?;
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            // Fetch the book and ensure it's available for borrowing
            var book = db.Books.FirstOrDefault(b => b.Id == bookId);
            if (book == null || book.StockQuantity == 0)
            {
                return HttpNotFound("Book not found or out of stock");
            }

            // Create a new Borrow record
            var borrow = new DataLayer.Borrow
            {
                UserId = userId.Value,
                BookId = book.Id,
                BorrowDate = DateTime.Now,
                DueDate = DateTime.Now.AddDays(14), // 14-day borrow period
                IsReturned = false,
                BorrowFee = (decimal?)Math.Round((book.Price * 0.25),2),
                LateFee = 0,
                Book = book
            };

            db.Borrows.Add(borrow);

            // Reduce stock quantity
            book.StockQuantity -= 1;

            db.SaveChanges();

            return RedirectToAction("ViewBorrowedBooks");
        }

        // View user's borrowed books
        public ActionResult ViewBorrowedBooks()
        {
            var userId = Session["UserId"] as int?;
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var borrowedBooks = db.Borrows
                .Where(b => b.UserId == userId && !b.IsReturned)
                .Select(b => new Models.Borrow
                {
                    Id = b.Id,
                    UserId = b.UserId,
                    BookId = b.BookId,
                    BorrowDate = b.BorrowDate,
                    ReturnDate = b.ReturnDate,
                    DueDate = b.DueDate,
                    IsReturned = b.IsReturned,
                    borrowFee = Math.Round(b.BorrowFee ?? 0,2),
                    LateFee = Math.Round(b.LateFee ?? 0,2),
                    Book = new Models.Book
                    {
                        Title = b.Book.Title
                    }
                })
                .ToList();

            return View(borrowedBooks);
        }

        // Mark book as returned
        public ActionResult ReturnBook(int borrowId)
        {
            var userId = Session["UserId"] as int?;
            if (!userId.HasValue)
            {
                return RedirectToAction("Login", "Account");
            }

            var borrow = db.Borrows.FirstOrDefault(b => b.Id == borrowId);
            if (borrow == null || borrow.IsReturned)
            {
                return HttpNotFound("Borrow record not found or already returned");
            }

            var daysLate = (DateTime.Now - borrow.DueDate).Days;
            if (daysLate > 0)
            {
                borrow.LateFee = Math.Round(daysLate * 0.25m); // Example late fee of $0.25 per day
            }

            // Mark the book as returned
            borrow.IsReturned = true;
            borrow.ReturnDate = DateTime.Now;

            // Increase stock quantity
            var book = db.Books.FirstOrDefault(b => b.Id == borrow.BookId);
            if (book != null)
            {
                book.StockQuantity += 1;
            }

            db.SaveChanges();

            var borrowedBooks = db.Borrows.Where(b => b.UserId == userId.Value && !b.IsReturned).Select(b => new Models.Borrow
            {
                Id = b.Id,
                UserId = b.UserId,
                BookId = b.BookId,
                BorrowDate = b.BorrowDate,
                ReturnDate = b.ReturnDate,
                DueDate = b.DueDate,
                IsReturned = b.IsReturned,
                borrowFee = Math.Round(b.BorrowFee ?? 0,2),
                LateFee = Math.Round(b.LateFee ?? 0,2),
                Book = new Models.Book
                {
                    Title = b.Book.Title
                },
            }).ToList();

            return PartialView("_BorrowedBooks", borrowedBooks);
        }

        public ActionResult ProceedToShipping()
        {
            Session["Process"] = "Borrow";

            return RedirectToAction("Shipping", "Shipping");
        }

    }
}