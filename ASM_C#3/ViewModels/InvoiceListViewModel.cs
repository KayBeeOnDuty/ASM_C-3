using ASM_C_3.Models;

namespace ASM_C_3.ViewModels
{
    public class InvoiceListViewModel
    {
        // Danh sách hóa đơn
        public IEnumerable<Invoice> Items { get; set; } = Enumerable.Empty<Invoice>();

        // Tổng số hóa đơn
        public int TotalCount { get; set; }

        // Phân trang
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;

        // Tính tổng số trang (bảo vệ chia 0)
        public int TotalPages =>
            (int)Math.Ceiling((double)(TotalCount == 0 ? 1 : TotalCount) / (PageSize <= 0 ? 10 : PageSize));

        // Từ khóa tìm kiếm (theo ID hoặc tên user)
        public string? Search { get; set; }

        // Trạng thái hóa đơn (Pending, Confirmed, Shipped, Completed, Cancelled, v.v.)
        public string? Status { get; set; }

        // Sắp xếp
        public string? SortBy { get; set; }
        public bool Desc { get; set; }

        // Dùng để hiển thị trang “My Invoices”
        public bool IsMyList { get; set; } = false;
    }
}
