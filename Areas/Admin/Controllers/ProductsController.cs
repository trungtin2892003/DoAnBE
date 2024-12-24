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
            // Lấy thông tin người dùng từ session
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");

            if (userInfo != null)
            {
                request.createdBy = request.updatedBy = userInfo.UserName;
            }
            else
            {
                return RedirectToAction("Login", "User");
            }
            if (!ModelState.IsValid)
            {
                // Trả về View cùng với thông báo lỗi
                ViewData["CAT_ID"] = new SelectList(_context.Categories, "CAT_ID", "CAT_ID", request.CAT_ID);
                return View(request);
            }
            // Kiểm tra dữ liệu đầu vào
            if (string.IsNullOrEmpty(request.Name))
            {
                ModelState.AddModelError("Name", "Tên sản phẩm không được để trống.");
            }
            else if (request.Name.Any(char.IsDigit)) // Kiểm tra nếu chứa số
            {
                ModelState.AddModelError("Name", "Tên sản phẩm không được chứa số.");
            }

            if (request.Price <= 0)
            {
                ModelState.AddModelError("Price", "Giá sản phẩm phải lớn hơn 0.");
            }
            // Khởi tạo đối tượng Product
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

            // Xử lý lưu ảnh
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

            // Lưu vào cơ sở dữ liệu
            _context.Add(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
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
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Product product, IFormFile? Avatar)
        {
            if (id != product.PRO_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Lấy sản phẩm từ cơ sở dữ liệu
                    var existingProduct = await _context.Products.FindAsync(id);
                    if (existingProduct == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin sản phẩm (trừ ảnh)
                    existingProduct.Name = product.Name;
                    existingProduct.Intro = product.Intro;
                    existingProduct.Price = product.Price;
                    existingProduct.DiscountPrice = product.DiscountPrice;
                    existingProduct.Unit = product.Unit;
                    existingProduct.Rate = product.Rate;
                    existingProduct.Description = product.Description;
                    existingProduct.Details = product.Details;
                    existingProduct.CAT_ID = product.CAT_ID;

                    // Xử lý ảnh nếu có upload
                    if (Avatar != null && Avatar.Length > 0)
                    {
                        // Thư mục lưu ảnh
                        var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot/Data/Product");

                        // Tạo thư mục nếu chưa tồn tại
                        if (!Directory.Exists(uploadsFolder))
                        {
                            Directory.CreateDirectory(uploadsFolder);
                        }

                        // Xóa ảnh cũ nếu có
                        if (!string.IsNullOrEmpty(existingProduct.Avatar))
                        {
                            var oldImagePath = Path.Combine(uploadsFolder, existingProduct.Avatar);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Lưu ảnh mới
                        var uniqueFileName = $"{Guid.NewGuid()}_{Path.GetFileName(Avatar.FileName)}";
                        var filePath = Path.Combine(uploadsFolder, uniqueFileName);
                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await Avatar.CopyToAsync(fileStream);
                        }

                        // Cập nhật đường dẫn ảnh
                        existingProduct.Avatar = uniqueFileName;
                    }

                    // Lưu thay đổi vào cơ sở dữ liệu
                    _context.Update(existingProduct);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
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
