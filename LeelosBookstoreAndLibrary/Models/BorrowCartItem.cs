using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class BorrowCartItem
    {
        public int Id { get; set; }
        public int BorrowCartId { get; set; } 
        public int BookId { get; set; } 

        public virtual BorrowCart BorrowCart { get; set; } 
        public virtual Book Book { get; set; }
    }
}
