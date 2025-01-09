using Azure.Core;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;
using ShopCake.Unity;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace ShopCake.Controllers
{
    public class CartController : Controller
    {
        private readonly CakeShopContext  _context;
        private readonly CartService _cartService;
        private readonly ILogger<CartController> _logger;

        public CartController(CakeShopContext context, CartService cartService, ILogger<CartController> logger)
        {
            _context = context;
            _cartService = cartService;
            _logger = logger;

        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("User_USE_ID") ?? (int?)null;
            // Kiểm tra null
            if (userId == null)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục!";
                return RedirectToAction("Index", "Home");
            }
                // Lấy giỏ hàng theo USE_ID
            var cartItems = await _cartService.GetCartDetailsByUserId(userId.Value);
            return View(cartItems);
        }



        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, string productName, decimal price, string productImage, int quantity = 1)
        {

            var userId = HttpContext.Session.GetInt32("User_USE_ID");
            if (userId.HasValue)
            {
                var cartItem = new Cart
                {
                    PRO_ID = productId,
                    ProductName = productName,
                    Price = price,
                    Quantity = quantity,
                    ProductImage = productImage,
                    USE_ID = userId.Value, // Lấy giá trị USE_ID từ session
                    TotalPrice = price * quantity
                };
                await _cartService.AddToCart(cartItem);
            }
            else
            {
                return RedirectToAction("Login", "User", new { area = "Admin" });
            }
            // Quay lại trang giỏ hàng hoặc trang sản phẩm
            return RedirectToAction("Index");
        }


        //Cập nhật số lượng 
        [HttpPost]
        public IActionResult UpdateQuantity([FromBody] UpdateQuantityDTO model)
        {
          
                if (model == null || model.CartId <= 0 || model.Quantity <= 0)
                {
                    return BadRequest("Invalid cart data.");
                }

                // Tìm sản phẩm trong giỏ hàng
                var cartItem = _context.Carts.FirstOrDefault(c => c.CAR_ID == model.CartId);
                if (cartItem == null)
                {
                    return NotFound("Item not found in the cart.");
                }

                // Cập nhật số lượng và tổng giá trị
                cartItem.Quantity = model.Quantity;
                cartItem.TotalPrice = cartItem.Quantity * cartItem.Price;  // Cập nhật lại tổng giá trị

               
                if (model.Quantity == 0) {
                    _context.Carts.Remove(cartItem);
                }
                _context.SaveChanges();
            // Trả về thông tin cập nhật
            return Ok(new { newTotalPrice = cartItem.TotalPrice.ToString("C") });
        }


        // GET: Admin/Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var CartItem = await _context.Carts
                .Include(p => p.USE_ID)
                .FirstOrDefaultAsync(m => m.CAR_ID == id);
            if (CartItem == null)
            {
                return NotFound();
            }

            return View(CartItem);
        }

        // POST: Admin/Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var CartItem = await _context.Carts.FindAsync(id);
            if (CartItem != null)
            {
                _context.Carts.Remove(CartItem);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("User_USE_ID") ?? (int?)null;
            // Kiểm tra null
            if (userId == null)
            {
                Console.WriteLine("User ID is null!");
                return RedirectToAction("Login", "User", new { area = "Admin" });
            }

            // Lấy giỏ hàng theo USE_ID
            var cartItems = await _cartService.GetCartDetailsByUserId(userId.Value);
            return View(cartItems);
        }

    }
}
