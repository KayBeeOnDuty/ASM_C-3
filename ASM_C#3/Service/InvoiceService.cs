using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.EntityFrameworkCore;

namespace ASM_C_3.Service
{
    public class InvoiceService : IInvoiceService
    {
        private readonly TraNgheDbContext _context;

        public InvoiceService(TraNgheDbContext context)
        {
            _context = context;
        }

        // Lấy toàn bộ hóa đơn, bao gồm chi tiết + user
        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Variant)
                .ThenInclude(v => v.Product)
                .ToListAsync();
        }

        // Lấy 1 hóa đơn cụ thể
        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Variant)
                .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        // Thêm hóa đơn + chi tiết
        public async Task AddAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
        }

        // Cập nhật hóa đơn (nếu cần)
        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

        // Xóa hóa đơn (và chi tiết)
        public async Task DeleteAsync(int id)
        {
            var invoice = await _context.Invoices
                .Include(i => i.InvoiceDetails)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);

            if (invoice != null)
            {
                if (invoice.InvoiceDetails.Any())
                    _context.InvoiceDetails.RemoveRange(invoice.InvoiceDetails);

                _context.Invoices.Remove(invoice);
                await _context.SaveChangesAsync();
            }
        }

    }
}
