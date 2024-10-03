using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class Borrow
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public int BookId { get; set; }
        public DateTime BorrowDate { get; set; } = DateTime.Now;
        public DateTime? ReturnDate { get; set; }
        public DateTime DueDate { get; set; }
        public bool IsReturned { get; set; } = false;

        // Navigation properties
        public virtual User User { get; set; }
        public virtual Book Book { get; set; }
        public decimal borrowFee { get; set; }
        public decimal LateFee { get; set; }
    }
}