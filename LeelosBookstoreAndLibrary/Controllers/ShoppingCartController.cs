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
                db.SaveChanges(); // Save to get the cart's ID
            }

            // Create a new ShoppingCartItem
            var shoppingCartItem = new ShoppingCartItem
            {
                BookId = bookId,
                Quantity = quantity
            };
            db.ShoppingCartItems.Add(shoppingCartItem);
            db.SaveChanges(); // Save to get the item's ID

            // Link the ShoppingCartItem to the ShoppingCart
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
                Price = (decimal)item.ShoppingCartItem.Price,
                Book = new LeelosBookstoreAndLibrary.Models.Book
                {
                    Title = item.Book.Title,
                    StockQuantity = item.Book.StockQuantity
                }
            }).ToList();

            return View(shoppingCartItemsModel);
        }

        [HttpPost]
        public ActionResult UpdateCart(Dictionary<int, int> quantities)
        {
            using (var db = new LeelosBookstoreEFDBEntities())
            {
                foreach (var quantity in quantities)
                {
                    var cartItem = db.ShoppingCartItems.Find(quantity.Key);
                    if (cartItem != null)
                    {
                        cartItem.Quantity = quantity.Value; // Update the quantity
                        db.Entry(cartItem).State = EntityState.Modified; // Mark as modified
                    }
                }

                db.SaveChanges(); // Save all changes
            }

            return RedirectToAction("ViewCart");
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
