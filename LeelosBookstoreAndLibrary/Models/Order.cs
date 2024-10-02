using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class Order
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public DateTime OrderDate { get; set; } = DateTime.Now;
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }

        public List<OrderItem> OrderItems { get; set; }
        //public int AddressId { get; set; }

        // Navigation property
        public virtual User User { get; set; }
        //public virtual ShippingInfo Address { get; set; }
    }
}