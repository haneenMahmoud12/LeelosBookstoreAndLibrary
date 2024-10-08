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
            try
            {
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
            catch (Exception e)
            {
                ModelState.AddModelError("", e.Message);
                TempData["ErrorMessage"] = e.Message;
                return RedirectToAction("Error", "Home");
            }
        }

        [HttpPost]
        public ActionResult ConfirmShipping(ShippingInfo shippingInfo)
        {
            int userId = (int)Session["UserId"];
            using (LeelosBookstoreEFDBEntities db = new LeelosBookstoreEFDBEntities())
            {
                try
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

                    db.SaveChanges();
                }
                catch (Exception e)
                {
                    ModelState.AddModelError("", e.Message);
                    TempData["ErrorMessage"] = e.Message;
                    return RedirectToAction("Error", "Home");
                }
            }

            return View("Shipping",shippingInfo);
        }



    }

}