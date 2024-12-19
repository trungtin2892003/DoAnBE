using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using ShopCake.Unity;
using System.IO;

namespace ShopCake.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class BannerController : Controller
    {
        private readonly CakeShopContext _context;
        private readonly IWebHostEnvironment _hostEnv;

        public BannerController(CakeShopContext context, IWebHostEnvironment hostEnv)
        {
            _context = context;
            _hostEnv = hostEnv;
        }

        // GET: Admin/Banner
        public async Task<IActionResult> Index()
        {
            var banners = await _context.Banners.ToListAsync();
            return View(banners);
        }

        // GET: Admin/Banner/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) return NotFound();

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

            var newBanner = new Banner
            {
                BAN_ID = banner.BAN_ID,
                Url = banner.Url,
                Title = banner.Title,
                createdBy = userInfo.UserName,
                updatedBy = userInfo.UserName,
            };

            if (ModelState.IsValid)
            {
                string? newImageFileName = null;

                if (banner.Image != null && banner.Image.Length > 0)
                {
                    newImageFileName = await SaveImageAsync(banner.Image, "Banner");
                }

                if (newImageFileName != null)
                {
                    newBanner.Image = newImageFileName;
                }

                _context.Add(newBanner);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }

            return View(banner);
        }

        // GET: Admin/Banner/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) return NotFound();

            return View(banner);
        }

        // POST: Admin/Banner/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] Banner banner, IFormFile? imageFile)
        {
            if (id != banner.BAN_ID) return NotFound();

            if (!ModelState.IsValid) return View(banner);

            try
            {
                if (imageFile != null && imageFile.Length > 0)
                {
                    if (!string.IsNullOrEmpty(banner.Image))
                    {
                        DeleteImage(banner.Image, "Banner");
                    }

                    banner.Image = await SaveImageAsync(imageFile, "Banner");
                }

                _context.Update(banner);
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!BannerExists(banner.BAN_ID)) return NotFound();
                throw;
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: Admin/Banner/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var banner = await _context.Banners.FindAsync(id);
            if (banner == null) return NotFound();

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
                if (!string.IsNullOrEmpty(banner.Image))
                {
                    DeleteImage(banner.Image, "Banner");
                }

                _context.Banners.Remove(banner);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        private bool BannerExists(int id)
        {
            return _context.Banners.Any(e => e.BAN_ID == id);
        }

        private async Task<string> SaveImageAsync(IFormFile imageFile, string folderName)
        {
            var folderPath = Path.Combine(_hostEnv.WebRootPath, "Data", folderName);

            if (!Directory.Exists(folderPath))
            {
                Directory.CreateDirectory(folderPath);
            }

            var fileName = $"{Guid.NewGuid()}{Path.GetExtension(imageFile.FileName)}";
            var filePath = Path.Combine(folderPath, fileName);

            using (var fileStream = new FileStream(filePath, FileMode.Create))
            {
                await imageFile.CopyToAsync(fileStream);
            }

            return fileName;
        }

        private void DeleteImage(string fileName, string folderName)
        {
            var filePath = Path.Combine(_hostEnv.WebRootPath, "Data", folderName, fileName);

            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
    }
}
