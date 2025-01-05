using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using ShopCake.Unity;
using System.IO;

namespace ShopCake.Areas.Admin.Controllers
{
    [Area("Admin")]
    //[Authorize(Roles = "Admin")]
    public class BannerController : Controller
    {
        private readonly CakeShopContext _context;
        private readonly IWebHostEnvironment _hostEnv;
        private const string BannerFolderName = "BannerImg";
<<<<<<< HEAD

=======
>>>>>>> 31a07424cebfad80acd5749d0cc95f66ef0415cc

       
        public BannerController(CakeShopContext context, IWebHostEnvironment hostEnv)
        {
            _context = context;
            _hostEnv = hostEnv;
        }

        // GET: Admin/Banner
        public async Task<IActionResult> Index()
        {
            var banners = await _context.Banners.AsNoTracking().ToListAsync();
            return View(banners);
        }

        // GET: Admin/Banner/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var banner = await _context.Banners.AsNoTracking().FirstOrDefaultAsync(b => b.BAN_ID == id);
            if (banner == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy banner.";
                return RedirectToAction(nameof(Index));
            }

            return View(banner);
        }

        // GET: Admin/Banner/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Banner/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] BannerDTO banner)
        {
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
            if (userInfo == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (!ModelState.IsValid || banner.Image == null || banner.Image.Length == 0)
            {
                ModelState.AddModelError("Image", "Ảnh không được để trống.");
                return View(banner);
            }

            var newBanner = new Banner
            {
                BAN_ID = banner.BAN_ID,
                Title = banner.Title,
                Url = banner.Url,
                createdBy = userInfo.UserName,
                updatedBy = userInfo.UserName
            };

            try
            {
                string newImageFileName = await SaveImageAsync(banner.Image);
                newBanner.Image = newImageFileName;

                _context.Add(newBanner);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Thêm banner thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi xảy ra khi lưu dữ liệu: {ex.Message}");
                return View(banner);
            }
        }

<<<<<<< HEAD

=======
       
>>>>>>> 31a07424cebfad80acd5749d0cc95f66ef0415cc
        // GET: Admin/Banner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy banner.";
                return RedirectToAction(nameof(Index));
            }

            // Map Banner to BannerDTO
            var bannerDto = new BannerDTO
            {
                BAN_ID = banner.BAN_ID,
                Title = banner.Title,
                Url = banner.Url,
                DisplayOrder = banner.DisplayOrder
            };

            return View(bannerDto);
        }
        // POST: Admin/Banner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] BannerDTO bannerDto)
        {
            if (id != bannerDto.BAN_ID)
            {
                TempData["ErrorMessage"] = "ID không khớp.";
                return RedirectToAction(nameof(Index));
            }

            if (!ModelState.IsValid)
            {
                TempData["ErrorMessage"] = "Thông tin không hợp lệ. Vui lòng kiểm tra lại.";
                return View(bannerDto);
            }

            var existingBanner = await _context.Banners.FindAsync(id);
            if (existingBanner == null)
            {
                TempData["ErrorMessage"] = "Banner không tồn tại.";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // Map DTO to entity
                existingBanner.Title = bannerDto.Title;
                existingBanner.Url = bannerDto.Url;
                existingBanner.DisplayOrder = bannerDto.DisplayOrder;

                if (bannerDto.Image != null && bannerDto.Image.Length > 0)
                {
                    // Delete old image if any
                    if (!string.IsNullOrEmpty(existingBanner.Image))
                    {
                        DeleteImage(existingBanner.Image);
                    }

                    // Save new image and update path
                    existingBanner.Image = await SaveImageAsync(bannerDto.Image);
                }

                _context.Update(existingBanner);
                await _context.SaveChangesAsync();

                TempData["SuccessMessage"] = "Cập nhật banner thành công.";
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BannerExists(bannerDto.BAN_ID))
                {
                    TempData["ErrorMessage"] = "Banner không tồn tại.";
                    return RedirectToAction(nameof(Index));
                }

                throw;
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Lỗi xảy ra khi cập nhật: {ex.Message}");
                return View(bannerDto);
            }
        }

        // GET: Admin/Banner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                TempData["ErrorMessage"] = "ID không hợp lệ.";
                return RedirectToAction(nameof(Index));
            }

            var banner = await _context.Banners.AsNoTracking().FirstOrDefaultAsync(b => b.BAN_ID == id);
            if (banner == null)
            {
                TempData["ErrorMessage"] = "Không tìm thấy banner.";
                return RedirectToAction(nameof(Index));
            }

            return View(banner);
        }

        // POST: Admin/Banner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var banner = await _context.Banners.FindAsync(id);

            if (banner != null)
            {
                try
                {
                    if (!string.IsNullOrEmpty(banner.Image))
                    {
                        DeleteImage(banner.Image);
                    }

                    _context.Banners.Remove(banner);
                    await _context.SaveChangesAsync();

                    TempData["SuccessMessage"] = "Xóa banner thành công.";
                }
                catch (Exception ex)
                {
                    TempData["ErrorMessage"] = $"Lỗi xảy ra khi xóa banner: {ex.Message}";
                }
            }
            else
            {
                TempData["ErrorMessage"] = "Không tìm thấy banner cần xóa.";
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BannerExists(int id)
        {
            return _context.Banners.Any(e => e.BAN_ID == id);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile)
        {
            var folderPath = Path.Combine(_hostEnv.WebRootPath, "Data", BannerFolderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            await using var fileStream = new FileStream(filePath, FileMode.Create);
            await imageFile.CopyToAsync(fileStream);

            return fileName;
        }

        private void DeleteImage(string fileName)
        {
            var filePath = Path.Combine(_hostEnv.WebRootPath, "Data", BannerFolderName, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}