using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using ShopCake.Unity;

namespace ShopCake.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductsController : Controller
    {
        private readonly CakeShopContext _context;
        private readonly IWebHostEnvironment _hostEnv;
        public ProductsController(CakeShopContext context, IWebHostEnvironment hostEnv)
        {
            _context = context;
            _hostEnv = hostEnv;
        }


        // GET: Admin/Products
        public IActionResult Index(string searchString)
        {
            // Lưu từ khóa tìm kiếm vào ViewData để hiển thị lại trong View
            ViewData["CurrentFilter"] = searchString;

            // Lấy toàn bộ danh sách sản phẩm
            var products = _context.Products.AsQueryable();

            // Nếu có từ khóa tìm kiếm, lọc danh sách theo tên sản phẩm
            if (!string.IsNullOrEmpty(searchString))
            {
                products = products.Where(p => p.Name.Contains(searchString));
            }

            // Trả về danh sách sản phẩm sau khi lọc
            return View(products.ToList());

        }
        // GET: Admin/Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PRO_ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Admin/Products/Create
        public IActionResult Create()
        {
            ViewData["CAT_ID"] = new SelectList(_context.Categories.OrderBy(c => c.Name), "CAT_ID", "Name");
            return View();
        }

        // POST: Admin/Products/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] ProductDTO request)
        {
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
            if (userInfo == null)
            {
                return RedirectToAction("Login", "User");
            }

            var product = new Product
            {
                PRO_ID = request.PRO_ID,
                CAT_ID = request.CAT_ID,
                Name = request.Name,
                Intro = request.Intro,
                Price = request.Price,
                DiscountPrice = request.DiscountPrice,
                Unit = request.Unit,
                Rate = request.Rate,
                Description = request.Description,
                Details = request.Details,
                createdBy = userInfo.UserName,
                updatedBy = userInfo.UserName,
            };

            if (ModelState.IsValid)
            {
                string? newImageFileName = null;

                if (request.Avatar != null && request.Avatar.Length > 0)
                {
                    var folderPath = Path.Combine(_hostEnv.WebRootPath, "Data", "Product");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var extension = Path.GetExtension(request.Avatar.FileName);
                    newImageFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(folderPath, newImageFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await request.Avatar.CopyToAsync(fileStream);
                    }
                }

                if (newImageFileName != null)
                {
                    product.Avatar = newImageFileName;
                }

                _context.Add(product);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            ViewData["CAT_ID"] = new SelectList(_context.Categories, "CAT_ID", "CAT_ID", product.CAT_ID);
            return View(product);
        }


        // GET: Admin/Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CAT_ID"] = new SelectList(_context.Categories, "CAT_ID", "CAT_ID", product.CAT_ID);
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PRO_ID,CAT_ID,Avatar,Name,Intro,Price,DiscountPrice,Unit,Rate,Description,Details")] Product product)
        {
            if (id != product.PRO_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.PRO_ID))
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
            ViewData["CAT_ID"] = new SelectList(_context.Categories, "CAT_ID", "CAT_ID", product.CAT_ID);
            return View(product);
        }

        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .FirstOrDefaultAsync(m => m.PRO_ID == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                _context.Products.Remove(product);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.PRO_ID == id);
        }
    }
}
