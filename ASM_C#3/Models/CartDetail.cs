using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class CartDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartDetailId { get; set; } // Mã dòng chi tiết giỏ hàng
        [Range(1, int.MaxValue, ErrorMessage = "Select a card")]
        public int CartId { get; set; } // Giỏ hàng chứa sản phẩm
        [Range(1, int.MaxValue, ErrorMessage = "Select a variant")]
        public int VariantId { get; set; } // Biến thể sản phẩm trong giỏ


        [Range(1, int.MaxValue)] public int Quantity { get; set; } = 1;
        [Range(0, int.MaxValue)] public int UnitPrice { get; set; }
        [Range(0, int.MaxValue)] public int SubTotal { get; set; }
        [ValidateNever] public Cart Cart { get; set; } = null!;
        [ValidateNever] public Variant Variant { get; set; } = null;
    }
}
