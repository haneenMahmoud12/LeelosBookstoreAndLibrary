using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class ShoppingCartItem
    {
        public int Id { get; set; }
        public int BookId { get; set; }
        public int Quantity { get; set; }

        // Navigation property
        public virtual Book Book { get; set; }

        public decimal Price { get; set; }
    }
}