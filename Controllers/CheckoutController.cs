using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ShopCake.Models;

namespace ShopCake.Controllers
{
    public class CheckoutController : Controller
    {
        private readonly CakeShopContext _context;
        private readonly ILogger<CartController> _logger;

        public CheckoutController(CakeShopContext context, ILogger<CartController> logger)
        {
            _context = context;
            _logger = logger;
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
                    var member = await _context.Members
                        .FirstOrDefaultAsync(m => m.USE_ID == userId.Value);

                    _logger.LogInformation($"Found member: {member?.MEM_ID}");

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
                        MEM_ID = member?.MEM_ID,
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
            var orderDetails = await _context.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product)
                .FirstOrDefaultAsync(o => o.ORD_ID == id);

            if (orderDetails == null)
            {
                return NotFound();
            }

            return View(orderDetails);
        }
        public async Task<IActionResult> Index()
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
