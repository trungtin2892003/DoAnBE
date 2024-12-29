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

        public CartController(CakeShopContext context)
        {
            _context = context;
        }

        public IActionResult Index()
        {
            var cart = HttpContext.Session.Get<List<CartDetail>>("Cart") ?? new List<CartDetail>();

            // Lấy danh sách sản phẩm từ cơ sở dữ liệu để hiển thị hình ảnh
            var productIds = cart.Select(c => c.PRO_ID).ToList();
            var products = _context.Products.Where(p => productIds.Contains(p.PRO_ID)).ToList();

            // Kết hợp thông tin từ giỏ hàng và cơ sở dữ liệu
            var cartItems = cart.Select(c => new CartDetail
            {
                PRO_ID = c.PRO_ID,
                ProductName = c.ProductName,
                Price = c.Price,
                ProductImage = products.FirstOrDefault(p => p.PRO_ID == c.PRO_ID)?.Avatar,
                Quantity = c.Quantity,
                Total = c.Total
            }).ToList();

            return View(cartItems);
        }

        [HttpPost]
        public IActionResult AddToCart(int productId, string productName, decimal price, string productImage, int quantity = 1)
        {
            // Lấy giỏ hàng từ session hoặc tạo mới nếu chưa có
            var cart = HttpContext.Session.Get<List<CartDetail>>("Cart") ?? new List<CartDetail>();

            // Kiểm tra xem sản phẩm đã có trong giỏ hàng chưa
            var item = cart.FirstOrDefault(x => x.PRO_ID == productId);

            // Nếu sản phẩm đã có trong giỏ, tăng số lượng
            if (item != null)
            {
                item.Quantity += quantity;
                item.Total = item.Price * item.Quantity; // Tính lại tổng giá trị của sản phẩm
            }
            else
            {
                // Nếu sản phẩm chưa có, thêm mới vào giỏ
                var newItem = new CartDetail
                {
                    PRO_ID = productId,
                    ProductName = productName,
                    Price = price,
                    Quantity = quantity,
                    ProductImage = productImage,
                    Total = price * quantity
                };
                cart.Add(newItem);
            }

            // Lưu giỏ hàng vào session
            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction("Index"); // Quay lại trang giỏ hàng
        }



        [HttpPost]
  
        public IActionResult UpdateCart(int productId, int quantity)
        {
            var cart = HttpContext.Session.Get<List<CartDetail>>("Cart") ?? new List<CartDetail>();

            var item = cart.FirstOrDefault(x => x.PRO_ID == productId);
            if (item != null)
            {
                item.Quantity = quantity;
                item.Total = item.Price * quantity;
            }

            HttpContext.Session.Set("Cart", cart);

            return RedirectToAction("Index");
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
