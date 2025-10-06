using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_3.Service
{
    public class ProductService : IProductService
    {
        private readonly TraNgheDbContext _context;

        public ProductService(TraNgheDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ sản phẩm, kèm danh mục, nhà cung cấp và các biến thể (size/topping)
        public async Task<IEnumerable<Product>> GetAllAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Variants)
                .ToListAsync();
        }

        // Lấy chi tiết 1 sản phẩm theo ID
        public async Task<Product?> GetByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == id);
        }

        // Thêm mới sản phẩm
        public async Task AddAsync(Product product)
        {
            _context.Products.Add(product);
            await _context.SaveChangesAsync();
        }

        // Cập nhật thông tin sản phẩm
        public async Task UpdateAsync(Product product)
        {
            _context.Products.Update(product);
            await _context.SaveChangesAsync();
        }

        // Xóa sản phẩm (và các variant liên quan)
        public async Task DeleteAsync(int id)
        {
            var product = await _context.Products
                .Include(p => p.Variants)
                .FirstOrDefaultAsync(p => p.ProductId == id);

            if (product != null)
            {
                // Xóa luôn các biến thể đi kèm (nếu có)
                if (product.Variants != null && product.Variants.Any())
                    _context.Variants.RemoveRange(product.Variants);

                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
        }

        // Tìm kiếm sản phẩm theo tên
        public async Task<IEnumerable<Product>> SearchByNameAsync(string keyword)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.Name.Contains(keyword))
                .ToListAsync();
        }

        // Lọc sản phẩm theo danh mục
        public async Task<IEnumerable<Product>> GetByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.Supplier)
                .Where(p => p.CategoryId == categoryId)
                .ToListAsync();
        }
    }
}
