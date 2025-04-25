using CingHuTang.Services.Vnpay;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System.Text.Json;

namespace CingHuTang.Controllers
{
    public class CheckoutController : Controller
    {
      

       
        public IActionResult PaymentCallbackVnpay()
        {
            
            return View();
        }
    }
}
