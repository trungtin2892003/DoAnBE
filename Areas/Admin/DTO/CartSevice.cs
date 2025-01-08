using ShopCake.Models;
using Microsoft.EntityFrameworkCore;


namespace ShopCake.Controllers
{
    public class CartService
    {
        private readonly CakeShopContext _context;

        public CartService(CakeShopContext context)
        {
            _context = context;
        }
        // Thêm sản phẩm vào giỏ hàng trong CSDL
        public async Task AddToCart(Cart item)
        {
            // Kiểm tra nếu sản phẩm đã có trong giỏ hàng của người dùng
            var existingItem = await _context.Carts
                .FirstOrDefaultAsync(c => c.USE_ID == item.USE_ID && c.PRO_ID == item.PRO_ID);

            if (existingItem != null)
            {
                // Nếu sản phẩm đã có, chỉ cần cập nhật số lượng và tổng giá trị
                existingItem.Quantity += item.Quantity;
                existingItem.TotalPrice = existingItem.Price * existingItem.Quantity;
                _context.Carts.Update(existingItem);
            }
            else
            {
                // Nếu sản phẩm chưa có, thêm mới
                await _context.Carts.AddAsync(item);
            }

            // Lưu thay đổi vào CSDL
            await _context.SaveChangesAsync();
        }

        // Lấy giỏ hàng theo UserId hoặc SessionId
        public async Task<List<Cart>> GetCartDetailsByUserId(int userId)
        {
            return await _context.Carts.Where(c => c.USE_ID == userId).ToListAsync();
        }

    }

}
