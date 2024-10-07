using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class BooksViewModel
    {
        public IEnumerable<Models.Book> Books { get; set; }
        public int CurrentPage { get; set; }
        public int PageSize { get; set; }
        public int TotalBooksCount { get; set; }
        public string SearchQuery { get; set; }
    }

}