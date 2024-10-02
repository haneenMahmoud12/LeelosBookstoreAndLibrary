using DataLayer;
using LeelosBookstoreAndLibrary.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeelosBookstoreAndLibrary.Controllers
{
    public class ShippingController : Controller
    {
        // GET: Shipping
        public ActionResult Shipping()
        {
            LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities();
            var userId = Session["UserId"] as int?;
            if (!userId.HasValue)
            {
                return RedirectToAction("Login");
            }

            var userAddress = db.Addresses.FirstOrDefault(addr => addr.UserId == userId);
            ShippingInfo shippingInfo = new ShippingInfo();
            if (userAddress != null)
            {
                shippingInfo = new ShippingInfo
                {
                    Address = userAddress.Address1,
                    City = userAddress.City,
                    Governorate = userAddress.Governorate,
                    ZipCode = userAddress.ZipCode,
                    Country = userAddress.Country,
                    PhoneNumber = userAddress.PhoneNumber
                };
            }

            return View(shippingInfo);
        }

        [HttpPost]
        public ActionResult ConfirmShipping(ShippingInfo shippingInfo)
        {
            int userId = (int)Session["UserId"];
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                var address = db.Addresses.FirstOrDefault(addr => addr.UserId == userId);
                if (address != null)
                {
                    // Update existing address
                    address.Address1 = shippingInfo.Address;
                    address.City = shippingInfo.City;
                    address.Governorate = shippingInfo.Governorate;
                    address.Country = shippingInfo.Country;
                    address.ZipCode = shippingInfo.ZipCode;
                    address.PhoneNumber = shippingInfo.PhoneNumber;
                }
                else
                {
                    // Create new address
                    address = new Address
                    {
                        UserId = userId,
                        Address1 = shippingInfo.Address,
                        City = shippingInfo.City,
                        Governorate = shippingInfo.Governorate,
                        Country = shippingInfo.Country,
                        ZipCode = shippingInfo.ZipCode,
                        PhoneNumber = shippingInfo.PhoneNumber
                    };
                    db.Addresses.Add(address);
                }

                try
                {
                    db.SaveChanges();
                }
                catch (DbEntityValidationException ex)
                {
                    foreach (var validationErrors in ex.EntityValidationErrors)
                    {
                        foreach (var validationError in validationErrors.ValidationErrors)
                        {
                            // Log the errors (for example, you can write them to a file or console)
                            Console.WriteLine($"Property: {validationError.PropertyName} Error: {validationError.ErrorMessage}");
                        }
                    }
                    // Optionally, return to the view with an error message
                    ModelState.AddModelError("", "There was a problem saving the address. Please check the details and try again.");
                    return View(shippingInfo); // Return the view with the original input to display errors
                }
            }

            return View("Shipping",shippingInfo);
        }



    }

}