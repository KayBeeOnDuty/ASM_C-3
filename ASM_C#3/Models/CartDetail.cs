using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class CartDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartDetailId { get; set; } // Mã dòng chi tiết giỏ hàng

        public int CartId { get; set; } // Giỏ hàng chứa sản phẩm
        public Cart? Cart { get; set; }

        public int VariantId { get; set; } // Biến thể sản phẩm trong giỏ
        public Variant? Variant { get; set; }

        public int Quantity { get; set; } // Số lượng sản phẩm
        public decimal Subtotal { get; set; } // Thành tiền dòng sản phẩm
    }
}
