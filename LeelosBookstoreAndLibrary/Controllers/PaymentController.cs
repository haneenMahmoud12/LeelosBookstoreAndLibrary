using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace LeelosBookstoreAndLibrary.Controllers
{
    public class PaymentController : Controller
    {
        // GET: Payment
        public ActionResult Payment()
        {
            return View();
        }

        // POST: ProcessPayment
        [HttpPost]
        public ActionResult ProcessPayment(string paymentMethod)
        {
            if (paymentMethod == "CashOnDelivery")
            {
                return RedirectToAction("Checkout", "Order");
            }

            ModelState.AddModelError("", "Invalid payment method.");
            return View("Payment");
        }
    }
}