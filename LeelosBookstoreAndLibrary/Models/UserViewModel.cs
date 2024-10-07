using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class UserViewModel
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }

        public ShippingInfo Address { get; set; }

        // List of orders
        public List<Order> Orders { get; set; }
        public IEnumerable<Borrow> BorrowedBooks { get; set; }

        // Paging Properties
        public int OrderPage { get; set; }
        public int OrderPageSize { get; set; }
        public int BorrowPage { get; set; }
        public int BorrowPageSize { get; set; }
        public int TotalOrders { get; set; }
        public int TotalBorrowedBooks { get; set; }
    }

}