using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCake.Models;
using System.Diagnostics;

namespace ShopCake.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly CakeShopContext _context;
        public HomeController(CakeShopContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public IActionResult Index()
        {
            ViewData["Categories"] = _context.Categories.AsNoTracking()
                .Include(x => x.Products!.OrderBy(y => y.Name))
                .OrderBy(c => c.DisplayOrder).ToList();
            ViewData["HotProduct"] = _context.Products.AsNoTracking()
               .Include(x => x.Category)
               .OrderBy(x => x.Price).ToList();
            return View();
        }
        public IActionResult Contact()
        {
            return View();
        }
        public IActionResult About()
        {
            return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
