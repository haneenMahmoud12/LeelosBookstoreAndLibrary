using DataLayer;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace LeelosBookstoreAndLibrary.Controllers
{
    public class ShoppingCartController : Controller
    {
        [HttpPost]
        public ActionResult AddToCart(int bookId, int quantity)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "Please login first to add books to your cart.";
                return RedirectToAction("Login", "Account"); 
            }

            int userId = (int)Session["UserId"];

            var cart = db.ShoppingCarts.FirstOrDefault(c => c.UserId == userId);
            if (cart == null)
            {
                cart = new ShoppingCart
                {
                    UserId = userId
                };
                db.ShoppingCarts.Add(cart);
                db.SaveChanges(); 
            }

            var book = db.Books.FirstOrDefault(b => b.Id == bookId);
            var shoppingCartItem = new ShoppingCartItem
            {
                BookId = bookId,
                Quantity = quantity,
                Price = (decimal) book.Price
            };
            db.ShoppingCartItems.Add(shoppingCartItem);
            //book.StockQuantity -= quantity;
            db.SaveChanges();

            db.ShoppingCart_ShoppingCartItems.Add(new ShoppingCart_ShoppingCartItems
            {
                ShoppingCartId = cart.Id,
                ShoppingCartItemId = shoppingCartItem.Id
            });

            db.SaveChanges();

            
            TempData["Message"] = "Book added to cart!";
            return RedirectToAction("BookDetails","Home", new { id = bookId });
        }

        public ActionResult ViewCart()
        {
            
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "Please login first to add books to your cart.";
                return RedirectToAction("Login", "Account"); 
            }
            int userId = (int)Session["UserId"];
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

            var cart = db.ShoppingCarts.FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                return View(new List<LeelosBookstoreAndLibrary.Models.ShoppingCartItem>());
            }

            
            var cartItems = db.ShoppingCart_ShoppingCartItems
                .Where(cs => cs.ShoppingCartId == cart.Id)
                .Select(cs => new
                {
                    ShoppingCartItem = cs.ShoppingCartItem,
                    Book = cs.ShoppingCartItem.Book 
        })
                .ToList();

            
            var shoppingCartItemsModel = cartItems.Select(item => new LeelosBookstoreAndLibrary.Models.ShoppingCartItem
            {
                Id = item.ShoppingCartItem.Id,
                BookId = item.ShoppingCartItem.BookId,
                Quantity = item.ShoppingCartItem.Quantity,
                Price = (decimal)item.ShoppingCartItem.Price*item.ShoppingCartItem.Quantity,
                Book = new LeelosBookstoreAndLibrary.Models.Book
                {
                    Title = item.Book.Title,
                    StockQuantity = item.Book.StockQuantity
                }
            }).ToList();

            return View(shoppingCartItemsModel);
        }

        [HttpPost]
        public JsonResult UpdateItemQuantity(int id, int quantity)
        {
            using (var db = new LeelosBookstoreEFDBEntities())
            {
                var cartItem = db.ShoppingCartItems.Find(id);
                if (cartItem != null)
                {
                    cartItem.Quantity = quantity;
                    cartItem.Price = (decimal)(cartItem.Book.Price * quantity);
                    db.Entry(cartItem).State = EntityState.Modified;
                    db.SaveChanges();
                }

                int userId = (int)Session["UserId"];
                var cart = db.ShoppingCarts.FirstOrDefault(c => c.UserId == userId);

                if (cart == null)
                {
                    return Json(new
                    {
                        updatedPrice = 0,
                        totalItems = 0,
                        totalPrice = 0
                    });
                }

                var cartItems = db.ShoppingCart_ShoppingCartItems
                    .Where(cs => cs.ShoppingCartId == cart.Id)
                    .Select(cs => cs.ShoppingCartItem)
                    .ToList();

                var totalItems = cartItems.Sum(i => i.Quantity);
                var totalPrice = cartItems.Sum(i => i.Price * i.Quantity);

                return Json(new
                {
                    updatedPrice = cartItem.Price.ToString(),
                    totalItems = totalItems,
                    totalPrice = totalPrice.ToString()
                });
            }
        }

        [HttpGet]
        public ActionResult RemoveFromCart(int id)
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            var item = db.ShoppingCartItems.Find(id);
            if (item != null)
            {
                var shoppingCartLinks = db.ShoppingCart_ShoppingCartItems
                                           .Where(link => link.ShoppingCartItemId == id )
                                           .ToList();

                foreach (var link in shoppingCartLinks)
                {
                    db.ShoppingCart_ShoppingCartItems.Remove(link);
                }

               /* var book = db.Books.FirstOrDefault(b => b.Id == item.Book.Id);
                book.StockQuantity += item.Quantity;*/
                db.ShoppingCartItems.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("ViewCart");
        }

        public ActionResult ProceedToShipping()
        {
            Session["Process"] = "Buy";
            return RedirectToAction("Shipping", "Shipping");
        }

    }
}
