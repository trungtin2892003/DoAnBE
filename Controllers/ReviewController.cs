using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using System.Text.Json;

namespace ShopCake.Controllers
{
    public class ReviewController : Controller
    {
        private readonly CakeShopContext _context;

        public ReviewController(CakeShopContext context)
        {
            _context = context;
        }

        // GET: Reviews
        public async Task<IActionResult> Index()
        {
            var reviews = await _context.Reviews
                .Include(r => r.Member)
                .Include(r => r.Product)
                .ToListAsync();
            return View(reviews);
        }

        // GET: Reviews/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Member)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.REV_ID == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // GET: Reviews/Create
        public IActionResult Create()
        {
            ViewData["MEM_ID"] = new SelectList(_context.Members, "MEM_ID", "Name");
            ViewData["PRO_ID"] = new SelectList(_context.Products, "PRO_ID", "Productname");
            return View();
        }
        public string GetUserNameFromCookie()
        {
            // Kiểm tra xem cookie "UserCredential" có tồn tại không
            if (Request.Cookies.ContainsKey("UserCredential"))
            {
                var cookieValue = Request.Cookies["UserCredential"];
                var loginDTO = JsonSerializer.Deserialize<LoginDTO>(cookieValue);

                // Trả về username
                return loginDTO?.Username;
            }

            return null; // Nếu cookie không tồn tại hoặc không có username
        }

        // POST: Reviews/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("REV_ID,MEM_ID,PRO_ID,Rate,Content,Productname")] Review review)
        {
            var userName = GetUserNameFromCookie();

            if (string.IsNullOrEmpty(userName))
            {
                // Nếu UserName không có (người dùng chưa đăng nhập), chuyển hướng đến trang đăng nhập
                return RedirectToAction("Login", "User", new { area = "Admin" });
            }

            // Lấy MEM_ID từ cơ sở dữ liệu dựa trên UserName
            var member = await _context.Members
                                        .FirstOrDefaultAsync(m => m.UserName == userName);


            if (member == null)
            {
                // Nếu không tìm thấy MEM_ID, bạn có thể thông báo lỗi hoặc chuyển hướng đến trang đăng nhập
               
                return RedirectToAction("Login", "User", new { area = "Admin" });
            }

            // Gán MEM_ID vào đối tượng Review và gán ReviewDate
            review.MEM_ID = member.MEM_ID;
            review.ReviewDate = DateTime.Now;

            if (ModelState.IsValid)
            {
                // Thêm đánh giá vào cơ sở dữ liệu
                _context.Add(review);
                await _context.SaveChangesAsync();

                // Chuyển hướng về trang chi tiết sản phẩm
                return RedirectToAction("Details", "Product", new { id = review.PRO_ID });
            }

            // Nếu không hợp lệ, quay lại trang Product/Details
            return RedirectToAction("Details", "Product", new { id = review.PRO_ID });
        }

    
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews
                .Include(r => r.Member)
                .Include(r => r.Product)
                .FirstOrDefaultAsync(m => m.REV_ID == id);

            if (review == null)
            {
                return NotFound();
            }

            return View(review);
        }

        // POST: Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.REV_ID == id);
        }
    }
}
