using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using ShopCake.Unity;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace ShopCake.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class TeamController : Controller
    {
        private readonly CakeShopContext _context;
        private readonly IWebHostEnvironment _hostEnv;

        public TeamController(CakeShopContext context, IWebHostEnvironment hostEnv)
        {
            _context = context;
            _hostEnv = hostEnv;
        }

        // GET: Admin/Team
        public async Task<IActionResult> Index()
        {
            return View(await _context.Teams.ToListAsync());
        }

        // GET: Admin/Team/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.TEAM_ID == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // GET: Admin/Team/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Admin/Team/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([FromForm] TeamDTO teamDTO)
        {
            var userInfo = HttpContext.Session.Get<AdminUser>("userInfo");
            if (userInfo == null)
            {
                return RedirectToAction("Login", "User");
            }

            if (ModelState.IsValid)
            {
                // Xử lý thông tin thành viên
                var team = new Team
                {
                    TEAM_ID = teamDTO.TEAM_ID,
                    Name = teamDTO.Name,
                    Position = teamDTO.Position,
                    FacebookUrl = teamDTO.FacebookUrl,
                    InstagramUrl = teamDTO.InstagramUrl,
                    createdBy=userInfo.createdBy ="Admin" ,
                    updatedBy=userInfo.updatedBy,
                };

                // Lưu ảnh nếu có
                if (teamDTO.PhotoUrl != null && teamDTO.PhotoUrl.Length > 0)
                {
                    var folderPath = Path.Combine(_hostEnv.WebRootPath, "Data", "Team");
                    if (!Directory.Exists(folderPath))
                    {
                        Directory.CreateDirectory(folderPath);
                    }

                    var extension = Path.GetExtension(teamDTO.PhotoUrl.FileName);
                    var newImageFileName = $"{Guid.NewGuid()}{extension}";
                    var filePath = Path.Combine(folderPath, newImageFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await teamDTO.PhotoUrl.CopyToAsync(fileStream);
                    }

                    // Cập nhật đường dẫn ảnh vào cơ sở dữ liệu
                    team.PhotoUrl = newImageFileName;
                }

                // Thêm thành viên vào cơ sở dữ liệu
                _context.Add(team);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            return View(teamDTO);
        }

        // GET: Admin/Team/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams.FindAsync(id);
            if (team == null)
            {
                return NotFound();
            }
            return View(team);
        }

        // POST: Admin/Team/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [FromForm] TeamDTO teamDTO)
        {
            if (id != teamDTO.TEAM_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var team = await _context.Teams.FindAsync(id);
                    if (team == null)
                    {
                        return NotFound();
                    }

                    // Cập nhật thông tin thành viên
                    team.Name = teamDTO.Name;
                    team.Position = teamDTO.Position;
                    team.FacebookUrl = teamDTO.FacebookUrl;
                    team.InstagramUrl = teamDTO.InstagramUrl;

                    // Lưu ảnh nếu có
                    if (teamDTO.PhotoUrl != null && teamDTO.PhotoUrl.Length > 0)
                    {
                        var folderPath = Path.Combine(_hostEnv.WebRootPath, "Data", "Team");
                        if (!Directory.Exists(folderPath))
                        {
                            Directory.CreateDirectory(folderPath);
                        }

                        var extension = Path.GetExtension(teamDTO.PhotoUrl.FileName);
                        var newImageFileName = $"{Guid.NewGuid()}{extension}";
                        var filePath = Path.Combine(folderPath, newImageFileName);

                        using (var fileStream = new FileStream(filePath, FileMode.Create))
                        {
                            await teamDTO.PhotoUrl.CopyToAsync(fileStream);
                        }

                        team.PhotoUrl = newImageFileName; // Cập nhật URL ảnh
                    }

                    _context.Update(team);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TeamExists(teamDTO.TEAM_ID))
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
            return View(teamDTO);
        }

        // GET: Admin/Team/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var team = await _context.Teams
                .FirstOrDefaultAsync(m => m.TEAM_ID == id);
            if (team == null)
            {
                return NotFound();
            }

            return View(team);
        }

        // POST: Admin/Team/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var team = await _context.Teams.FindAsync(id);
            if (team != null)
            {
                _context.Teams.Remove(team);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TeamExists(int id)
        {
            return _context.Teams.Any(e => e.TEAM_ID == id);
        }
    }
}
