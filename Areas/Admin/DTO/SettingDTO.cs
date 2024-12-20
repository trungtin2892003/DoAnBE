using Microsoft.EntityFrameworkCore;
using ShopCake.Models;

namespace ShopCake.Areas.Admin.DTO
{
    public class SettingDTO
    {
        private readonly CakeShopContext _context;

        public SettingDTO(CakeShopContext context)
        {
            _context = context;
        }

        // Lấy cài đặt dựa trên key
        public async Task<string?> GetSettingValueAsync(string key)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Name == key);
            return setting?.Value;
        }

        // Cập nhật giá trị cài đặt
        public async Task UpdateSettingValueAsync(string key, string value)
        {
            var setting = await _context.Settings.FirstOrDefaultAsync(s => s.Name == key);
            if (setting != null)
            {
                setting.Value = value;
            }
            else
            {
                // Nếu chưa tồn tại, thêm mới
                setting = new Setting { Name = key, Value = value };
                _context.Settings.Add(setting);
            }
            await _context.SaveChangesAsync();
        }
    }

}
