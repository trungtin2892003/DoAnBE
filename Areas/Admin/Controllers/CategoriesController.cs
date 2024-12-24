using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using ShopCake.Unity;

namespace ShopCake.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CategoriesController : Controller
    {
        private readonly CakeShopContext _context;

        public CategoriesController(CakeShopContext context)
        {
            _context = context;
        }

        // GET: Admin/Categories
        public async Task<IActionResult> Index()
        {
            return View(await _context.Categories.OrderBy(c => c.DisplayOrder).ToListAsync());
        }

        // GET: Admin/Categories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CAT_ID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // GET: Admin/Categories/Create
        public IActionResult Create()
        {

            return View();
        }

        // POST: Admin/Categories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Category category)
        {
            // Lấy thông tin người dùng từ session
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");

            if (userInfo != null)
            {
                category.createdBy=category.updatedBy = userInfo.UserName;
            }
            else
            {
                // Nếu không có thông tin người dùng trong session, trả về lỗi hoặc redirect.
                return RedirectToAction("Login", "User");
            }
            if (int.TryParse(category.Name, out _))
            {
                ModelState.AddModelError("Name", "Tên danh mục không được là số.");
            }
            // Kiểm tra các điều kiện khác nếu cần
            if (_context.Categories.Any(c => c.Name == category.Name))
            {
                ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
            }

            // Kiểm tra tính hợp lệ của model
            if (ModelState.IsValid)
            {
                _context.Add(category);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(category);
        }


        // GET: Admin/Categories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }
            return View(category);
        }

        // POST: Admin/Categories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] CategoryDTO categoryDTO)
        {
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");

            if (userInfo == null)
            {
                return RedirectToAction("Login", "User", new { area = "Admin" });
            }

            // Kiểm tra nếu id không hợp lệ
            var category = await _context.Categories.FindAsync(id);
            if (category == null)
            {
                return NotFound();
            }

            // Kiểm tra dữ liệu tên danh mục
            if (int.TryParse(categoryDTO.name, out _))
            {
                ModelState.AddModelError("Name", "Tên danh mục không được là số.");
            }
            // Kiểm tra nếu tên danh mục đã tồn tại
            if (_context.Categories.Any(c => c.Name == categoryDTO.name && c.CAT_ID != id))
            {
                ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
            }

            if (ModelState.IsValid)
            {
                // Chuyển đổi từ DTO sang entity
                category.Name = categoryDTO.name;
                category.updatedBy = userInfo.UserName;
                category.updatedDate = DateTime.Now; // Cập nhật thời gian sửa đổi

                try
                {
                    _context.Update(category); // Cập nhật entity
                    await _context.SaveChangesAsync(); // Lưu thay đổi vào DB
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CategoryExists(category.CAT_ID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index)); // Quay lại danh sách
            }

            return View(categoryDTO); // Trả về View nếu có lỗi
        }

        private bool CategoryExists(int id)
        {
            return _context.Categories.Any(e => e.CAT_ID == id);
        }


        // GET: Admin/Categories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var category = await _context.Categories
                .FirstOrDefaultAsync(m => m.CAT_ID == id);
            if (category == null)
            {
                return NotFound();
            }

            return View(category);
        }

        // POST: Admin/Categories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var category = await _context.Categories.FindAsync(id);
            if (category != null)
            {
                _context.Categories.Remove(category);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private bool CategoryExists(int id)
        //{
        //    return _context.Categories.Any(e => e.CAT_ID == id);
        //}
    }
}
