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
          
            if (request.Price <= 0)
            {
                ModelState.AddModelError("Price", "Giá sản phẩm phải lớn hơn 0.");
            }
            else if(request.Price >= 1000){
                ModelState.AddModelError("Price", "Giới hạn giá là 1000.");
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
                createdBy = userInfo.DisplayName,
                updatedBy = userInfo.DisplayName,
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
            ViewData["CAT_ID"] = new SelectList(_context.Categories.OrderBy(c => c.Name), "CAT_ID", "Name");
            return View(product);
        }

        // POST: Admin/Products/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] ProductDTO productDTO)
        {
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");

            if (userInfo == null)
            {
                return RedirectToAction("Login", "User", new { area = "Admin" });
            }

            if (id != productDTO.PRO_ID)
            {
                return NotFound();
            }

            var product = await _context.Products.FindAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    // Cập nhật thông tin cơ bản
                    product.Name = productDTO.Name;
                    product.Price = productDTO.Price;
                    product.Quantity = productDTO.Quantity;
                    product.Unit = productDTO.Unit;
                    product.Intro = productDTO.Intro;
                    product.DiscountPrice = productDTO.DiscountPrice;
                    product.Rate = productDTO.Rate;
                    product.Description = productDTO.Description;
                    product.CAT_ID = productDTO.CAT_ID;
                    product.updatedBy = userInfo.UserName;
                    product.updatedDate = DateTime.Now;

                    // Xử lý avatar mới nếu được tải lên
                    if (productDTO.Avatar != null && productDTO.Avatar.Length > 0)
                    {
                        var folderPath = Path.Combine(_hostEnv.WebRootPath, "Data", "Product");

                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        // Xóa avatar cũ nếu tồn tại
                        if (!string.IsNullOrEmpty(product.Avatar))
                        {
                            var oldImagePath = Path.Combine(folderPath, product.Avatar);
                            if (System.IO.File.Exists(oldImagePath))
                            {
                                System.IO.File.Delete(oldImagePath);
                            }
                        }

                        // Lưu avatar mới
                        var extension = Path.GetExtension(productDTO.Avatar.FileName);
                        var newImageFileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(folderPath, newImageFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await productDTO.Avatar.CopyToAsync(fileStream);
                        }

                        product.Avatar = newImageFileName; // Cập nhật tên file mới
                    }

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

            ViewData["CAT_ID"] = new SelectList(_context.Categories, "CAT_ID", "Name", productDTO.CAT_ID);
            return View(productDTO);
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
