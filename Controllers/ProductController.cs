using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCake.Models;

namespace ShopCake.Controllers
{
    public class ProductController : Controller
    {
        private readonly CakeShopContext _context;
        public ProductController(CakeShopContext context)
        {
            _context = context;

        }
        public IActionResult Index()
        {
            return View();
        }
        public IActionResult Details(long id)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu dựa trên ID
            var product = _context.Products.AsTracking().
                Include(x => x.Category)
                .FirstOrDefault(p => p.PRO_ID == id);
            if (product == null)
            {
                // Nếu không tìm thấy sản phẩm, chuyển hướng đến trang lỗi
                return NotFound();
            }

            ViewData["Title"] = "Chi tiet san pham";
            return View(product);
        }
    }
}
