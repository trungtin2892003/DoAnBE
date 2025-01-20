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
            ViewData["HotProduct"] = _context.Products.AsNoTracking()
              .Include(x => x.Category)
              .OrderBy(x => x.Price).ToList();
           
            // Gửi dữ liệu sang View
            ViewData["Categories"] = categories;
            ViewData["Products"] = products.ToList();

            return View();
        }
        public IActionResult Details(long id)
        {
            // Lấy thông tin sản phẩm từ cơ sở dữ liệu dựa trên ID
            var product = _context.Products
                .AsNoTracking()
                .Include(p => p.Category)
                .Include(p => p.Review)
                    .ThenInclude(r => r.Member)
                .FirstOrDefault(p => p.PRO_ID == id);

            // Nếu không tìm thấy sản phẩm, trả về trang lỗi
            if (product == null)
            {
                return NotFound();
            }

            // Lấy danh sách sản phẩm tương tự theo danh mục
            var relatedProducts = _context.Products
                .AsNoTracking()
                .Where(p => p.CAT_ID == product.CAT_ID && p.PRO_ID != id) // Cùng danh mục, khác ID hiện tại
                .OrderBy(p => Guid.NewGuid()) // Random thứ tự
                .Take(4) // Lấy 4 sản phẩm
                .ToList();

            // Lấy review đầu tiên (nếu có)
            var firstReview = product.Review.FirstOrDefault();
            var memberId = firstReview?.Member?.MEM_ID;

            // Gửi dữ liệu sang View
            ViewData["Title"] = "Chi tiết sản phẩm";
            ViewData["FirstReviewMemberId"] = memberId; // ID của thành viên đánh giá đầu tiên (nếu có)
            ViewData["RelatedProducts"] = relatedProducts; // Sản phẩm tương tự

            // Trả về View với sản phẩm
            return View(product);
        }



    }
}
