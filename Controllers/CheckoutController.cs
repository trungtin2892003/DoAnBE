using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCake.Models;

namespace ShopCake.Controllers
{
    public class CheckoutController : Controller
    {
        public async Task<IActionResult> Index()
        {
           
            return View();
        }
    }
}
