﻿using System;
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
    public class ReviewsController : Controller
    {
        private readonly CakeShopContext _context;

        public ReviewsController(CakeShopContext context)
        {
            _context = context;
        }

        // GET: Admin/Reviews
        public async Task<IActionResult> Index()
        {
            var cakeShopContext = _context.Reviews.Include(r => r.Member).Include(r => r.Product);
            return View(await cakeShopContext.ToListAsync());
        }

        // GET: Admin/Reviews/Details/5
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

        // GET: Admin/Reviews/Create
        public IActionResult Create()
        {
            ViewData["MEM_ID"] = new SelectList(_context.Members, "MEM_ID", "MEM_ID");
            ViewData["PRO_ID"] = new SelectList(_context.Products, "PRO_ID", "PRO_ID");
            return View();
        }

        // POST: Admin/Reviews/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("REV_ID,MEM_ID,PRO_ID,Rate,Content,ReviewDate,CreatedDate,createdBy,updatedDate,updatedBy")] Review review)
        {
            if (ModelState.IsValid)
            {
                _context.Add(review);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["MEM_ID"] = new SelectList(_context.Members, "MEM_ID", "MEM_ID", review.MEM_ID);
            ViewData["PRO_ID"] = new SelectList(_context.Products, "PRO_ID", "PRO_ID", review.PRO_ID);
            return View(review);
        }

        // GET: Admin/Reviews/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var review = await _context.Reviews.FindAsync(id);
            if (review == null)
            {
                return NotFound();
            }
            ViewData["MEM_ID"] = new SelectList(_context.Members, "MEM_ID", "MEM_ID", review.MEM_ID);
            ViewData["PRO_ID"] = new SelectList(_context.Products, "PRO_ID", "PRO_ID", review.PRO_ID);
            return View(review);
        }

        // POST: Admin/Reviews/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("REV_ID,MEM_ID,PRO_ID,Rate,Content,ReviewDate,CreatedDate,createdBy,updatedDate,updatedBy")] Review review)
        {
            if (id != review.REV_ID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(review);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ReviewExists(review.REV_ID))
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
            ViewData["MEM_ID"] = new SelectList(_context.Members, "MEM_ID", "MEM_ID", review.MEM_ID);
            ViewData["PRO_ID"] = new SelectList(_context.Products, "PRO_ID", "PRO_ID", review.PRO_ID);
            return View(review);
        }

        // GET: Admin/Reviews/Delete/5
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

        // POST: Admin/Reviews/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var review = await _context.Reviews.FindAsync(id);
            if (review != null)
            {
                _context.Reviews.Remove(review);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ReviewExists(int id)
        {
            return _context.Reviews.Any(e => e.REV_ID == id);
        }
    }
}
