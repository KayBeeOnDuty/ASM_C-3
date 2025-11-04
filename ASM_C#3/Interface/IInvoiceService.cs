using System.Collections.Generic;
using System.Threading.Tasks;
using ASM_C_3.Models;

namespace ASM_C_3.Interface
{
    public interface IInvoiceService
    {
        // ===== CRUD CƠ BẢN =====
        Task<IEnumerable<Invoice>> GetAllAsync();
        Task<Invoice?> GetByIdAsync(int id);
        Task AddAsync(Invoice invoice);
        Task UpdateAsync(Invoice invoice);
        Task DeleteAsync(int id);

        // ===== TRUY VẤN MỞ RỘNG =====
        Task<(IEnumerable<Invoice> Items, int TotalCount)> QueryAsync(
            string? search, string? status, string? sortBy, bool desc, int page, int pageSize);

        Task<IEnumerable<Invoice>> GetByUserAsync(int userId);

        // ===== THAY ĐỔI TRẠNG THÁI =====
        Task<bool> ConfirmAsync(int id);
        Task<bool> MarkShippedAsync(int id);
        Task<bool> CancelAsync(int id, string? reason);
        Task<bool> MarkCompletedByUserAsync(int id, int userId);
        Task<bool> RequestCancelAsync(int id, int userId, string? reason);
    }
}
