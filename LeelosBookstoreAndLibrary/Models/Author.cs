using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class Author
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "First Name is required")]
        [StringLength(30, ErrorMessage = "Name cannot be longer than 30 characters")]
        public string FirstName { get; set; }
        [Required(ErrorMessage = "Last Name is required")]
        [StringLength(30, ErrorMessage = "Name cannot be longer than 30 characters")]
        public string LastName { get; set; }
        [StringLength(500, ErrorMessage = "Biography cannot be longer than 500 characters")]
        public string Biography { get; set; }
        public byte[] AuthorImage { get; set; }  // For storing the image as a byte array
        public string ImageMimeType { get; set; }  // For storing the image MIME type
    }
}