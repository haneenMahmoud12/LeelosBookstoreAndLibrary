using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class ShoppingCartShoppingCartItem
    {
        public int ShoppingCartId { get; set; }
        public int ShoppingCartItemId { get; set; }

        // Navigation properties
        public virtual ShoppingCart ShoppingCart { get; set; }
        public virtual ShoppingCartItem ShoppingCartItem { get; set; }
    }
}