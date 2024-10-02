using System.ComponentModel.DataAnnotations;

namespace LeelosBookstoreAndLibrary.Models
{
    public class ShippingInfo
    {
        [Required]
        public string Address { get; set; }

        [Required]
        public string Governorate { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string Country { get; set; }

        [Required]
        [Phone]
        [StringLength(15, ErrorMessage = "Phone number cannot be longer than 15 characters.")]
        public string PhoneNumber { get; set; }
    }
}
