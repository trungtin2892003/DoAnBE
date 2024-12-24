using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using ShopCake.Models;

namespace ShopCake.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class CartsController : Controller
    {
        private readonly CakeShopContext _context;

        public CartsController(CakeShopContext context)
        {
            _context = context;
        }

        // GET: Admin/Carts
        public async Task<IActionResult> Index()
        {
            var cakeShopContext = _context.Carts.Include(c => c.Member).Include(c => c.User);
            return View(await cakeShopContext.ToListAsync());
        }

        // GET: Admin/Carts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Member)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CAR_ID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // GET: Admin/Carts/Create
        public IActionResult Create()
        {
            ViewData["MEM_ID"] = new SelectList(_context.Members, "MEM_ID", "MEM_ID");
            ViewData["USE_ID"] = new SelectList(_context.Orders, "ORD_ID", "ORD_ID");
            return View();
        }

        // POST: Admin/Carts/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CAR_ID,USE_ID,MEM_ID,CartDate,CustomerName,Phone,Address,TotalPrice,Discount,PaymentMethod,Note,Status,CreatedDate,createdBy,updatedDate,updatedBy")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cart);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MEM_ID"] = new SelectList(_context.Members, "MEM_ID", "MEM_ID", cart.MEM_ID);
            ViewData["USE_ID"] = new SelectList(_context.Orders, "ORD_ID", "ORD_ID", cart.USE_ID);
            return View(cart);
        }

        // GET: Admin/Carts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts.FindAsync(id);
            if (cart == null)
            {
                return NotFound();
            }
            ViewData["MEM_ID"] = new SelectList(_context.Members, "MEM_ID", "MEM_ID", cart.MEM_ID);
            ViewData["USE_ID"] = new SelectList(_context.Orders, "ORD_ID", "ORD_ID", cart.USE_ID);
            return View(cart);
        }

        // POST: Admin/Carts/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CAR_ID,USE_ID,MEM_ID,CartDate,CustomerName,Phone,Address,TotalPrice,Discount,PaymentMethod,Note,Status,CreatedDate,createdBy,updatedDate,updatedBy")] Cart cart)
        {
            if (id != cart.CAR_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cart);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CartExists(cart.CAR_ID))
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
            ViewData["MEM_ID"] = new SelectList(_context.Members, "MEM_ID", "MEM_ID", cart.MEM_ID);
            ViewData["USE_ID"] = new SelectList(_context.Orders, "ORD_ID", "ORD_ID", cart.USE_ID);
            return View(cart);
        }

        // GET: Admin/Carts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cart = await _context.Carts
                .Include(c => c.Member)
                .Include(c => c.User)
                .FirstOrDefaultAsync(m => m.CAR_ID == id);
            if (cart == null)
            {
                return NotFound();
            }

            return View(cart);
        }

        // POST: Admin/Carts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cart = await _context.Carts.FindAsync(id);
            if (cart != null)
            {
                _context.Carts.Remove(cart);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CartExists(int id)
        {
            return _context.Carts.Any(e => e.CAR_ID == id);
        }
    }
}
