using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public enum PayMethod
    {
        Cash, CreditCard, DebitCard, MobilePayment
    }
    public enum OrderStatus
    {
        Pending = 0,        // User created, awaiting staff/admin confirmation
        Confirmed = 1,      // Confirmed by staff/admin
        Shipped = 2,        // Marked as shipped by staff/admin
        Completed = 3,      // User confirmed received
        CancelRequested = 4,// User requested cancel (before shipped)
        Cancelled = 5       // Cancelled by admin/staff (or approved cancel)
    }
    public class Invoice
    {

        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceId { get; set; } // Mã hóa đơn
        [Required]
        public int UserId { get; set; } // Người mua hàng
        public PayMethod? PayMethod { get; set; } // Phương thức thanh toán
        [Range(0, int.MaxValue, ErrorMessage = "TotalPrice must be >= 0")]
        public int TotalPrice { get; set; }
        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Ngày tạo hóa đơn

        public OrderStatus Status { get; set; } = OrderStatus.Pending; // Trạng thái đơn hàng
        public bool IsDeleted { get; set; } = false;
        [StringLength(500)] public string? CancelReason { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? CompletedAt { get; set; }
        public DateTime? CancelledAt { get; set; }
        [ValidateNever] public User User { get; set; } = null!; // EF populated
        [ValidateNever] public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
        public decimal TotalAmount { get; set; } // Tổng tiền hóa đơn

    }
}
