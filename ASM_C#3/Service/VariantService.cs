using ASM_C_3.Data;
using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_C_3.Service
{
    public class VariantService : IVariantService
    {
        private readonly TraNgheDbContext _context;

        public VariantService(TraNgheDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Variant>> GetAllAsync()
        {
            return await _context.Variants
                .Include(v => v.Product)
                .ThenInclude(p => p.Category)
                .ToListAsync();
        }

        public async Task<Variant?> GetByIdAsync(int id)
        {
            return await _context.Variants
                .Include(v => v.Product)
                .ThenInclude(p => p.Category)
                .FirstOrDefaultAsync(v => v.VariantId == id);
        }

        public async Task AddAsync(Variant variant)
        {
            _context.Variants.Add(variant);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Variant variant)
        {
            _context.Variants.Update(variant);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int id)
        {
            var variant = await _context.Variants.FindAsync(id);
            if (variant != null)
            {
                _context.Variants.Remove(variant);
                await _context.SaveChangesAsync();
            }
        }
    }
}
