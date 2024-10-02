using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class CheckoutViewModel
    {
        public User User { get; set; }
        public ShippingInfo ShippingInfo { get; set; }
        public List<ShoppingCartItem> cartItems { get; set; } 
        public decimal TotalPrice { get; set; }
        public string PaymentMethod { get; set; } = "Cash on Delivery";
        public int AddressId { get; set; }
        /* public string CardNumber { get; set; } 
         public int PaymentId { get; set; }*/
    }

}