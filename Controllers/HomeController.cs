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
               .OrderBy(c => c.DisplayOrder)  
               .Include(x => x.Products)  
               .ToList();  

         
            ViewData["HotProduct"] = _context.Products.AsNoTracking()
               .Include(x => x.Category)  // T?i Category c?a m?i s?n ph?m
               .OrderBy(x => x.Price)  // S?p x?p s?n ph?m theo giá
              .ToList();  // L?y danh sách s?n ph?m sau khi s?p x?p

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
