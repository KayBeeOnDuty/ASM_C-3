using ASM_C_3.Data;
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

        // ===== CƠ BẢN =====
        public async Task<IEnumerable<Invoice>> GetAllAsync()
        {
            return await _context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Variant)
                        .ThenInclude(v => v.Product)
                .ToListAsync();
        }

        public async Task<Invoice?> GetByIdAsync(int id)
        {
            return await _context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Variant)
                        .ThenInclude(v => v.Product)
                .FirstOrDefaultAsync(i => i.InvoiceId == id);
        }

        public async Task AddAsync(Invoice invoice)
        {
            _context.Invoices.Add(invoice);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(Invoice invoice)
        {
            _context.Invoices.Update(invoice);
            await _context.SaveChangesAsync();
        }

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

        // ===== MỞ RỘNG =====

        // Truy vấn có phân trang, lọc, tìm kiếm, sắp xếp
        public async Task<(IEnumerable<Invoice> Items, int TotalCount)> QueryAsync(
            string? search, string? status, string? sortBy, bool desc, int page, int pageSize)
        {
            var query = _context.Invoices
                .Include(i => i.User)
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Variant)
                        .ThenInclude(v => v.Product)
                .AsQueryable();

            // --- Lọc theo trạng thái ---
            if (!string.IsNullOrEmpty(status) && Enum.TryParse<OrderStatus>(status, true, out var parsedStatus))
            {
                query = query.Where(i => i.Status == parsedStatus);
            }

            // --- Tìm kiếm theo tên người dùng hoặc ID hóa đơn ---
            if (!string.IsNullOrEmpty(search))
            {
                query = query.Where(i =>
                    i.User.FullName.Contains(search) ||
                    i.InvoiceId.ToString().Contains(search));
            }

            // --- Sắp xếp ---
            query = sortBy?.ToLower() switch
            {
                "price" => desc ? query.OrderByDescending(i => i.TotalPrice) : query.OrderBy(i => i.TotalPrice),
                "date" => desc ? query.OrderByDescending(i => i.CreatedDate) : query.OrderBy(i => i.CreatedDate),
                _ => query.OrderByDescending(i => i.InvoiceId)
            };

            // --- Tổng số bản ghi ---
            var totalCount = await query.CountAsync();

            // --- Phân trang ---
            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            return (items, totalCount);
        }

        // Lấy hóa đơn theo người dùng
        public async Task<IEnumerable<Invoice>> GetByUserAsync(int userId)
        {
            return await _context.Invoices
                .Include(i => i.InvoiceDetails)
                    .ThenInclude(d => d.Variant)
                        .ThenInclude(v => v.Product)
                .Where(i => i.UserId == userId)
                .OrderByDescending(i => i.CreatedDate)
                .ToListAsync();
        }

        // ===== CẬP NHẬT TRẠNG THÁI =====

        public async Task<bool> ConfirmAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return false;

            invoice.Status = OrderStatus.Confirmed;
            invoice.ConfirmedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkShippedAsync(int id)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return false;

            invoice.Status = OrderStatus.Shipped;
            invoice.ShippedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CancelAsync(int id, string? reason)
        {
            var invoice = await _context.Invoices.FindAsync(id);
            if (invoice == null) return false;

            invoice.Status = OrderStatus.Cancelled;
            invoice.CancelReason = reason ?? "Cancelled by admin";
            invoice.IsDeleted = true;
            invoice.CancelledAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> MarkCompletedByUserAsync(int id, int userId)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.UserId == userId);
            if (invoice == null) return false;

            invoice.Status = OrderStatus.Completed;
            invoice.CompletedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> RequestCancelAsync(int id, int userId, string? reason)
        {
            var invoice = await _context.Invoices
                .FirstOrDefaultAsync(i => i.InvoiceId == id && i.UserId == userId);
            if (invoice == null) return false;

            invoice.Status = OrderStatus.CancelRequested;
            invoice.CancelReason = reason ?? "User requested cancellation";
            await _context.SaveChangesAsync();
            return true;
        }
    }
}
