using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class Variant
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VariantId { get; set; } // Mã biến thể

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty; // Tên biến thể (VD: Size M, Less Sugar)

        [Range(0, 999999)]
        public decimal Price { get; set; } // Giá biến thể

        public bool IsAvailable { get; set; } = true; // Còn bán hay không

        public int ProductId { get; set; } // Mã sản phẩm cha
        public Product? Product { get; set; }

        public ICollection<CartDetail>? CartDetails { get; set; } // Dòng giỏ hàng chứa biến thể này
        public ICollection<InvoiceDetail>? InvoiceDetails { get; set; } // Dòng hóa đơn chứa bi
    }
}
