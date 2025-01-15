using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCake.Areas.Admin.DTO;
using ShopCake.Models;



namespace ShopCake.Controllers
{
    public class CartController : Controller
    {
        private readonly CakeShopContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(CakeShopContext context, ILogger<CartController> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<IActionResult> Index()
        {
            var userId = HttpContext.Session.GetInt32("User_USE_ID");
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục!";
                return RedirectToAction("Index", "Home");
            }

            // Lấy giỏ hàng theo USE_ID
            var cartItems = await _context.Carts
                .Where(c => c.USE_ID == userId.Value)
                .ToListAsync();
            return View(cartItems);
        }

        [HttpPost]
        public async Task<IActionResult> ProcessCheckout([FromBody] OrderCreateModel model)
        {
            try
            {
                var userId = HttpContext.Session.GetInt32("User_USE_ID");
                _logger.LogInformation($"Processing checkout for user: {userId}");

                if (!userId.HasValue)
                {
                    _logger.LogWarning("User not logged in");
                    return RedirectToAction("Login", "User");
                }

                using var transaction = await _context.Database.BeginTransactionAsync();
                try
                {
                    // Lấy cart items
                    var cartItems = await _context.Carts
                        .Where(c => c.USE_ID == userId.Value)
                        .ToListAsync();

                    _logger.LogInformation($"Found {cartItems.Count} items in cart");

                    if (!cartItems.Any())
                    {
                        return BadRequest("Giỏ hàng trống");
                    }

                    // Tính tổng tiền từ giỏ hàng
                    decimal totalAmount = cartItems.Sum(c => c.TotalPrice);
                    _logger.LogInformation($"Total amount: {totalAmount}");

                    // Tạo order
                  

                    var order = new Order
                    {
                        CustomerName = model.CustomerName,
                        Phone = model.Phone,
                        Address = model.Address,
                        OrderDate = DateTime.Now,
                        TotalPrice = (int)totalAmount,
                        PaymentMethod = model.PaymentMethod,
                        Status = 1,
                        IsPaid = false,
                        USE_ID = userId.Value,
                       
                        CreatedDate = DateTime.Now,
                        createdBy = userId.Value.ToString(),
                        updatedDate = DateTime.Now,
                        updatedBy = userId.Value.ToString()
                    };

                    _context.Orders.Add(order);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Created order with ID: {order.ORD_ID}");

                    // Tạo order details
                    var orderDetails = cartItems.Select(cartItem => new OrderDetail
                    {
                        ORD_ID = order.ORD_ID,
                        PRO_ID = cartItem.PRO_ID,
                        Quantity = cartItem.Quantity,
                        Price = cartItem.Price,
                        DiscountPrice = 0,
                        CreatedDate = DateTime.Now,
                        createdBy = userId.Value.ToString(),
                        updatedDate = DateTime.Now,
                        updatedBy = userId.Value.ToString()
                    }).ToList();

                    await _context.OrderDetails.AddRangeAsync(orderDetails);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation($"Added {orderDetails.Count} order details");

                    foreach (var cartItem in cartItems)
                    {
                        var product = await _context.Products.FirstOrDefaultAsync(p => p.PRO_ID == cartItem.PRO_ID);
                        if (product != null)
                        {
                            product.SlOrder += cartItem.Quantity;  // Tăng số lượng đã bán
                            _context.Products.Update(product);
                        }

                    }

                    // Xóa cart items
                    _context.Carts.RemoveRange(cartItems);
                    await _context.SaveChangesAsync();
                    _logger.LogInformation("Removed all cart items");

                    await transaction.CommitAsync();
                    _logger.LogInformation("Transaction committed successfully");
                    return Ok(new { orderId = order.ORD_ID });
                }
                catch (Exception ex)
                {
                    await transaction.RollbackAsync();
                    _logger.LogError(ex, "Error in transaction: {Message}", ex.Message);
                    if (ex.InnerException != null)
                    {
                        _logger.LogError("Inner exception: {Message}", ex.InnerException.Message);
                    }
                    throw;
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error in ProcessCheckout");
                var errorMessage = ex.InnerException?.Message ?? ex.Message;
                return StatusCode(500, new
                {
                    message = "Có lỗi xảy ra khi xử lý đơn hàng. Vui lòng thử lại.",
                    error = errorMessage
                });
            }
        }

        // GET: Cart/OrderSuccess/5
        public async Task<IActionResult> OrderSuccess(int id)
        {
            var order = await _context.Orders
    .FirstOrDefaultAsync(o => o.ORD_ID == id);

            if (order == null)
            {
                return NotFound();
            }

            // Then fetch the order details separately
          

            return View( order);

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
                    USE_ID = userId.Value,
                    Status = "New",
                    CartDate = DateTime.Now
                };
                await _context.Carts.AddAsync(cartItem);
                await _context.SaveChangesAsync();
            }
            else
            {
                return RedirectToAction("Login", "User", new { area = "" });
            }
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
            cartItem.Status = "Updated";  // Nếu cần cập nhật status

            if (model.Quantity == 0)
            {
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

        // Thêm action Checkout
        public async Task<IActionResult> Checkout()
        {
            var userId = HttpContext.Session.GetInt32("User_USE_ID");
            if (!userId.HasValue)
            {
                TempData["ErrorMessage"] = "Vui lòng đăng nhập để tiếp tục!";
                return RedirectToAction("Login", "User");
            }

            // Lấy cart items từ database
            var cartItems = await _context.Carts
                .Where(c => c.USE_ID == userId.Value)
                .ToListAsync();

            if (!cartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng trống";
                return RedirectToAction("Index", "Home");
            }

            return View(cartItems);
        }
    }
}