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
                if (Session["Process"] != null && Session["Process"].Equals("Borrow"))
                {
                    return RedirectToAction("BorrowCheckout","Order");
    }
                else if (Session["Process"] != null && Session["Process"].Equals("Buy"))
                {
                    return RedirectToAction("Checkout", "Order");
                }
            }

            ModelState.AddModelError("", "Invalid payment method.");
            return View("Payment");
        }
    }
}