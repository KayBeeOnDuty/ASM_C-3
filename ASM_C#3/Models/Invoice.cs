using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class Invoice
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceId { get; set; } // Mã hóa đơn

        public int UserId { get; set; } // Người mua hàng
        public User? User { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày tạo hóa đơn
        public decimal TotalAmount { get; set; } // Tổng tiền hóa đơn

        public ICollection<InvoiceDetail>? InvoiceDetails { get; set; } // Chi tiết hóa đơn
    }
}
