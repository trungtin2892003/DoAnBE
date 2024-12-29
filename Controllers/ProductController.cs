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
        public IActionResult Index(int? categoryId, string searchQuery)
        {
            // Lấy danh sách danh mục
            var categories = _context.Categories.ToList();

            // Lấy danh sách sản phẩm
            var products = _context.Products.AsQueryable();

            // Nếu chọn danh mục, lọc sản phẩm theo danh mục
            if (categoryId.HasValue)
            {
                products = products.Where(p => p.CAT_ID == categoryId.Value);
            }

            // Nếu có từ khóa tìm kiếm, lọc sản phẩm theo từ khóa
            if (!string.IsNullOrEmpty(searchQuery))
            {
                products = products.Where(p => p.Name.Contains(searchQuery));
            }

            // Gửi dữ liệu sang View
            ViewData["Categories"] = categories;
            ViewData["Products"] = products.ToList();

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
