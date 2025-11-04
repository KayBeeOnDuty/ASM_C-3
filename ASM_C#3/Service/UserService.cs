using ASM_C_3.Interface;
using ASM_C_3.Data;
using ASM_C_3.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace ASM_C_3.Service
{
    public class UserService : IUserService
    {
        private readonly TraNgheDbContext _context;

        public UserService(TraNgheDbContext context)
        {
            _context = context;
        }

        // Lấy tất cả user, kèm danh sách hóa đơn (Invoices)
        public async Task<IEnumerable<User>> GetAllAsync()
        {
            return await _context.AppUsers
                .Include(u => u.Invoices) // 🔹 Gắn hóa đơn
                .ToListAsync();
        }

        // Lấy user theo ID, kèm danh sách hóa đơn
        public async Task<User?> GetByIdAsync(int id)
        {
            return await _context.AppUsers
                .Include(u => u.Invoices)
                .FirstOrDefaultAsync(u => u.UserId == id);
        }

        // Thêm user mới
        public async Task AddAsync(User user)
        {
            _context.AppUsers.Add(user);
            await _context.SaveChangesAsync();
        }

        // Cập nhật user
        public async Task UpdateAsync(User user)
        {
            _context.AppUsers.Update(user);
            await _context.SaveChangesAsync();
        }

        // Xóa user theo ID
        public async Task DeleteAsync(int id)
        {
            var user = await _context.AppUsers.FindAsync(id);
            if (user == null) return;

            _context.AppUsers.Remove(user);
            await _context.SaveChangesAsync();
        }
    }
}
