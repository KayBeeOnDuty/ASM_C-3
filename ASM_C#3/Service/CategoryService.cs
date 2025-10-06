using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_3.Service
{
    public class CategoryService : ICategoryService
    {
        private readonly TraNgheDbContext _context;

        public CategoryService(TraNgheDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ danh mục, kèm theo danh sách sản phẩm thuộc mỗi danh mục
        public async Task<IEnumerable<Category>> GetAllAsync()
        {
            return await _context.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.Variants) // lấy thêm các size/topping của sản phẩm
                .ToListAsync();
        }

        // Lấy 1 danh mục cụ thể theo ID, kèm sản phẩm
        public async Task<Category?> GetByIdAsync(int id)
        {
            return await _context.Categories
                .Include(c => c.Products)
                    .ThenInclude(p => p.Variants)
                .FirstOrDefaultAsync(c => c.CategoryId == id);
        }

        // Thêm danh mục mới
        public async Task AddAsync(Category category)
        {
            _context.Categories.Add(category);
            await _context.SaveChangesAsync();
        }

        // Cập nhật danh mục
        public async Task UpdateAsync(Category category)
        {
            _context.Categories.Update(category);
            await _context.SaveChangesAsync();
        }

        // Xóa danh mục (nếu cần, xóa luôn sản phẩm trong danh mục)
        public async Task DeleteAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.CategoryId == id);

            if (category != null)
            {
                // Nếu danh mục có sản phẩm, xóa luôn (hoặc bạn có thể kiểm tra trước khi xóa)
                if (category.Products != null && category.Products.Any())
                {
                    _context.Products.RemoveRange(category.Products);
                }

                _context.Categories.Remove(category);
                await _context.SaveChangesAsync();
            }
        }

        // Tìm danh mục theo tên (dùng khi lọc hoặc tìm kiếm)
        public async Task<IEnumerable<Category>> SearchByNameAsync(string keyword)
        {
            return await _context.Categories
                .Include(c => c.Products)
                .Where(c => c.Name.Contains(keyword))
                .ToListAsync();
        }
    }
}
