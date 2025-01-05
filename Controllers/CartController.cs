using Microsoft.AspNetCore.Cors.Infrastructure;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using ShopCake.Models;
using ShopCake.Unity;
using System;
using System.Collections.Generic;
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
            var userId = HttpContext.Session.GetInt32("USE_ID") ?? (int?)null;

            // Kiểm tra null
            if (userId == null)
            {
                Console.WriteLine("User ID is null!");
                return RedirectToAction("Login", "User", new { area = "Admin" });
            }

            try
            {
                // Lấy giỏ hàng theo USE_ID
                var cartItems = await _cartService.GetCartDetailsByUserId(userId.Value);
                return View(cartItems);
            }
            catch (Exception ex)
            {
                // Log lỗi để debug
                Console.WriteLine($"Error while fetching cart details: {ex.Message}");
                return StatusCode(500, "Internal Server Error");
            }
        }



        // Thêm sản phẩm vào giỏ hàng
        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId, string productName, decimal price, string productImage, int quantity = 1)
        {

            var userId = HttpContext.Session.GetInt32("USE_ID");
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
                Console.WriteLine("USE_ID không tồn tại trong session.");
                return RedirectToAction("Login", "User", new { area = "Admin" });
            }

            // Lưu sản phẩm vào giỏ hàng trong CSDL
           
            _logger.LogInformation("User added item to cart.");
            // Quay lại trang giỏ hàng hoặc trang sản phẩm
            return RedirectToAction("Index");
        }

  
        [HttpPost]
        public IActionResult UpdateQuantity(int productId, int quantity)
        {
            // Lấy giỏ hàng từ Session
            var cart = HttpContext.Session.Get<List<CartDetail>>("Cart") ?? new List<CartDetail>();

            // Tìm sản phẩm trong giỏ hàng
            var item = cart.FirstOrDefault(x => x.PRO_ID == productId);
            if (item != null)
            {
                // Cập nhật số lượng và tổng tiền
                item.Quantity = quantity;
                item.Total = item.Price * quantity;

                // Lưu lại giỏ hàng vào Session
                HttpContext.Session.Set("Cart", cart);

                // Tính tổng tiền giỏ hàng
                var cartTotal = cart.Sum(x => x.Total);

                // Trả về kết quả JSON
                return Json(new
                {
                    success = true,
                    newTotal = item.Total,
                    cartTotal = cartTotal
                });
            }

            return Json(new { success = false });
        }


        [HttpPost]
        public IActionResult RemoveFromCart(int productId)
        {
            var cart = HttpContext.Session.Get<List<CartDetail>>("Cart") ?? new List<CartDetail>();

            var item = cart.FirstOrDefault(x => x.PRO_ID == productId);
            if (item != null)
            {
                cart.Remove(item);
            }

            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction("Index");
        }
    }
}
