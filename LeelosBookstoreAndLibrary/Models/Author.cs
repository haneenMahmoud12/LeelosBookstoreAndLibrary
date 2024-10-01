using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class Author
    {
        public int Id { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string Biography { get; set; }
        public byte[] AuthorImage { get; set; }  // For storing the image as a byte array
        public string ImageMimeType { get; set; }  // For storing the image MIME type
    }
}