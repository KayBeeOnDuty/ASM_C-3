using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class InvoiceDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceDetailId { get; set; } // Mã dòng chi tiết hóa đơn

        public int InvoiceId { get; set; }
        public Invoice? Invoice { get; set; } // Hóa đơn cha

        public int VariantId { get; set; }
        public Variant? Variant { get; set; } // Biến thể được bán

        public int Quantity { get; set; } // Số lượng sản phẩm
        public decimal UnitPrice { get; set; } // Giá tại thời điểm bán
        public decimal Subtotal { get; set; } // Thành tiền (Quantity * UnitPrice)
    }
}
