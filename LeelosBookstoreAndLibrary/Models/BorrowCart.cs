using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class BorrowCart
    {
        public int Id { get; set; }
        public int UserId { get; set; } 
        public virtual User User { get; set; } 
        public virtual ICollection<BorrowCartItem> BorrowCartItems { get; set; } 
    }
}