using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace LeelosBookstoreAndLibrary.Models
{
    public class Book
    {
        public int Id { get; set; }
        [Required(ErrorMessage = "Title is required")]
        public string Title { get; set; }
        public string Description { get; set; }
        [Required(ErrorMessage = "Author is required")]
        public int AuthorId { get; set; }  // Foreign Key for Author
        [Required(ErrorMessage = "Genre is required")]
        public string Genre { get; set; }
        [Required(ErrorMessage = "Price is required")]
        public float Price { get; set; }
        [Required(ErrorMessage = "Stock Quantity is required")]
        public int StockQuantity { get; set; } = 0;  // Default to 0
        public int? Rating { get; set; }  // Nullable, constrained to values between 1 and 5
        [Required(ErrorMessage = "Publisher is required")]
        public int PublisherId { get; set; }  // Foreign Key for Publisher
        [Required(ErrorMessage = "Date published is required")]
        public DateTime DatePublished { get; set; }
        public int? NumberOfPages { get; set; }
        public byte[] ImageData { get; set; }  // For storing the image as a byte array
        public string ImageMimeType { get; set; }  // For storing the image MIME type

        // Navigation properties
        public virtual Author Author { get; set; }
        public virtual Publisher Publisher { get; set; }
    }
}