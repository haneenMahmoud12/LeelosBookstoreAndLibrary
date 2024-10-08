using DataLayer;
using System;
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
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
                {
                    if (Session["UserId"] == null)
                    {
                        TempData["ErrorMessage"] = "Please login first to add books to your cart.";
                        return RedirectToAction("Login", "Account");
                    }

                    int userId = (int)Session["UserId"];

                    // Find or create the shopping cart for the user
                    var cart = db.ShoppingCarts.FirstOrDefault(c => c.UserId == userId);
                    if (cart == null)
                    {
                        cart = new ShoppingCart
                        {
                            UserId = userId
                        };
                        db.ShoppingCarts.Add(cart);
                        db.SaveChanges();  // Save to generate cart.Id
                    }

                    // Find the book by its ID
                    var book = db.Books.FirstOrDefault(b => b.Id == bookId);
                    if (book == null)
                    {
                        TempData["ErrorMessage"] = "Book not found.";
                        return RedirectToAction("Index", "Home");
                    }

                    // Check if the book is already in the cart
                    var cartItemLink = db.ShoppingCart_ShoppingCartItems.FirstOrDefault(i => i.ShoppingCartId == cart.Id && i.ShoppingCartItem.BookId == bookId);

                    if (cartItemLink != null)
                    {
                        // Book is already in cart, increment the quantity
                        var shoppingCartItem = db.ShoppingCartItems.FirstOrDefault(i => i.Id == cartItemLink.ShoppingCartItemId);
                        if (shoppingCartItem != null)
                        {
                            shoppingCartItem.Quantity += quantity;
                        }
                    }
                    else
                    {
                        // Book is not in the cart, add new ShoppingCartItem
                        var newShoppingCartItem = new ShoppingCartItem
                        {
                            BookId = bookId,
                            Quantity = quantity,
                            Price = Math.Round((decimal)book.Price, 2)
                        };

                        db.ShoppingCartItems.Add(newShoppingCartItem);
                        db.SaveChanges();  // Save to generate ShoppingCartItem.Id

                        // Link the new item with the ShoppingCart using ShoppingCart_ShoppingCartItems
                        db.ShoppingCart_ShoppingCartItems.Add(new ShoppingCart_ShoppingCartItems
                        {
                            ShoppingCartId = cart.Id,
                            ShoppingCartItemId = newShoppingCartItem.Id
                        });
                    }

                    // Save the changes to the database
                    db.SaveChanges();

                    // Success message
                    TempData["Message"] = "Book added to cart!";
                    return RedirectToAction("BookDetails", "Home", new { id = bookId });
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Home");
                }
            }
        }

        public ActionResult ViewCart()
        {
            try
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
                    Price = Math.Round((decimal)item.ShoppingCartItem.Price, 2),
                    Book = new LeelosBookstoreAndLibrary.Models.Book
                    {
                        Title = item.Book.Title,
                        StockQuantity = item.Book.StockQuantity
                    }
                }).ToList();

                return View(shoppingCartItemsModel);
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public JsonResult UpdateItemQuantity(int id, int quantity)
        {
            try
            {
                using (var db = new LeelosBookstoreEFDBEntities())
                {
                    var cartItem = db.ShoppingCartItems.Find(id);
                    if (cartItem != null)
                    {
                        cartItem.Quantity = quantity;
                        cartItem.Price = Math.Round((decimal)(cartItem.Book.Price), 2);
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
                        totalPrice = Math.Round((double)totalPrice, 2).ToString()
                    });
                }
            }
            catch (Exception ex)
            {
                return Json(new
                {
                    success = false,
                    message = "An error occurred while updating the item quantity. Please try again."
                });
            }
        }

        [HttpGet]
        public ActionResult RemoveFromCart(int id)
        {
            try
            {
                LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
                var item = db.ShoppingCartItems.Find(id);
                if (item != null)
                {
                    var shoppingCartLinks = db.ShoppingCart_ShoppingCartItems
                                               .Where(link => link.ShoppingCartItemId == id)
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
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        public ActionResult ProceedToShipping()
        {
            try
            {
                Session["Process"] = "Buy";
                return RedirectToAction("Shipping", "Shipping");
            }
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

    }
}
