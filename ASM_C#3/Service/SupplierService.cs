using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_3.Service
{
    public class SupplierService : ISupplierService
    {
        private readonly TraNgheDbContext _context;

        public SupplierService(TraNgheDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Supplier>> GetAllAsync()
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                .ThenInclude(p => p.Category)
                .Include(s => s.Products)
                .ThenInclude(p => p.Variants)
                .ToListAsync();
        }

        public async Task<Supplier?> GetByIdAsync(int id)
        {
            return await _context.Suppliers
                .Include(s => s.Products)
                    .ThenInclude(p => p.Category)
                .Include(s => s.Products)
                    .ThenInclude(p => p.Variants)
                .FirstOrDefaultAsync(s => s.SupplierId == id);
        }

        public async Task AddAsync(Supplier supplier)
        {
            _context.Suppliers.Add(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Supplier supplier)
        {
            _context.Suppliers.Update(supplier);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var supplier = await _context.Suppliers.FindAsync(id);
            if (supplier != null)
            {
                _context.Suppliers.Remove(supplier);
                await _context.SaveChangesAsync();
            }
        }

        public async Task<bool> SupplierExistsAsync(int id)
        {
            return await _context.Suppliers.AnyAsync(s => s.SupplierId == id);
        }
    }
}
