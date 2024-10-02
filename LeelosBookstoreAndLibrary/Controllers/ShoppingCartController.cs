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

            // Check if the user is logged in
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "Please login first to add books to your cart.";
                return RedirectToAction("Login", "Account"); // Redirect to the login page or the appropriate action
            }

            int userId = (int)Session["UserId"];

            // Check if the user already has a shopping cart
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

            // Create a new ShoppingCartItem
            var book = db.Books.FirstOrDefault(b => b.Id == bookId);
            var shoppingCartItem = new ShoppingCartItem
            {
                BookId = bookId,
                Quantity = quantity,
                Price = (decimal) book.Price
            };
            db.ShoppingCartItems.Add(shoppingCartItem);
            db.SaveChanges();

            db.ShoppingCart_ShoppingCartItems.Add(new ShoppingCart_ShoppingCartItems
            {
                ShoppingCartId = cart.Id,
                ShoppingCartItemId = shoppingCartItem.Id
            });

            db.SaveChanges();

            // Pass success message to the view using TempData or ViewBag
            TempData["Message"] = "Book added to cart!";
            return RedirectToAction("BookDetails","Home", new { id = bookId });
        }

        public ActionResult ViewCart()
        {
            // Check if the user is logged in
            if (Session["UserId"] == null)
            {
                TempData["ErrorMessage"] = "Please login first to add books to your cart.";
                return RedirectToAction("Login", "Account"); // Redirect to the login page or the appropriate action
            }
            int userId = (int)Session["UserId"];
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();

            var cart = db.ShoppingCarts.FirstOrDefault(c => c.UserId == userId);

            if (cart == null)
            {
                return View(new List<LeelosBookstoreAndLibrary.Models.ShoppingCartItem>());
            }

            // Fetch shopping cart items with their associated books using a join
            var cartItems = db.ShoppingCart_ShoppingCartItems
                .Where(cs => cs.ShoppingCartId == cart.Id)
                .Select(cs => new
                {
                    ShoppingCartItem = cs.ShoppingCartItem,
                    Book = cs.ShoppingCartItem.Book // Ensure Book is fetched in the same query
        })
                .ToList();

            // Now map the results back to your ShoppingCartItem model
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
                    // Update the quantity and price of the cart item
                    cartItem.Quantity = quantity;
                    cartItem.Price = (decimal)(cartItem.Book.Price * quantity); // Calculate new price
                    db.Entry(cartItem).State = EntityState.Modified;
                    db.SaveChanges();
                }

                // Get the user's shopping cart
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

                // Get all the items in the user's shopping cart
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
                // Remove any reference to this item in ShoppingCart_ShoppingCartItems
                var shoppingCartLinks = db.ShoppingCart_ShoppingCartItems
                                           .Where(link => link.ShoppingCartItemId == id )
                                           .ToList();

                foreach (var link in shoppingCartLinks)
                {
                    db.ShoppingCart_ShoppingCartItems.Remove(link);
                }

                // Now remove the ShoppingCartItem
                db.ShoppingCartItems.Remove(item);
                db.SaveChanges();
            }

            return RedirectToAction("ViewCart");
        }

    }
}
