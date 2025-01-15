using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using ShopCake.Unity;
using System;
using System.Linq;
using System.Threading.Tasks;

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
            var categories = await _context.Categories.OrderBy(c => c.DisplayOrder).ToListAsync();
            return View(categories);
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] Category category)
        {
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");

            if (userInfo != null)
            {
                category.createdBy = category.updatedBy = userInfo.UserName;
            }
            else
            {
                return RedirectToAction("Login", "User");
            }

            if (int.TryParse(category.Name, out _))
            {
                ModelState.AddModelError("Name", "Tên danh mục không được là số.");
            }

            if (_context.Categories.Any(c => c.Name == category.Name))
            {
                ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
            }

            if (_context.Categories.Any(c => c.DisplayOrder == category.DisplayOrder))
            {
                ModelState.AddModelError("DisplayOrder", "DisplayOrder đã tồn tại. Vui lòng chọn giá trị khác.");
            }

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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Category category)
        {
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");

            if (userInfo == null)
            {
                return RedirectToAction("Login", "User", new { area = "Admin" });
            }

            if (id != category.CAT_ID)
            {
                return NotFound();
            }

            if (int.TryParse(category.Name, out _))
            {
                ModelState.AddModelError("Name", "Tên danh mục không được là số.");
            }

            if (_context.Categories.Any(c => c.Name == category.Name && c.CAT_ID != id))
            {
                ModelState.AddModelError("Name", "Tên danh mục đã tồn tại.");
            }

            if (_context.Categories.Any(c => c.DisplayOrder == category.DisplayOrder && c.CAT_ID != id))
            {
                ModelState.AddModelError("DisplayOrder", "DisplayOrder đã tồn tại. Vui lòng chọn giá trị khác.");
            }

            if (ModelState.IsValid)
            {
                category.updatedBy = userInfo.UserName;
                category.updatedDate = DateTime.Now;

                try
                {
                    _context.Update(category);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Categories.Any(e => e.CAT_ID == id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }

            return View(category);
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
                var hasProducts = _context.Products.Any(p => p.CAT_ID == id);
                if (hasProducts)
                {
                    TempData["ErrorMessage"] = "Cannot delete this category because it has associated products.";
                    return RedirectToAction(nameof(Index));
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
                TempData["SuccessMessage"] = "Category deleted successfully.";
            }
            else
            {
                TempData["ErrorMessage"] = "Category not found.";
            }

            return RedirectToAction(nameof(Index));
        }
    }
}
