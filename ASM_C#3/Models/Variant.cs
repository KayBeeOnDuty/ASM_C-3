using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class Variant
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int VariantId { get; set; } // Mã biến thể

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty; // Tên biến thể (VD: Size M, Less Sugar)

        [Range(0, int.MaxValue, ErrorMessage = "Price must be >= 0")]
        public decimal Price { get; set; } // Giá biến thể

        public bool IsAvailable { get; set; } = true; // Còn bán hay không

        [Range(1, int.MaxValue, ErrorMessage = "Select a product")]
        public int ProductId { get; set; } // Mã sản phẩm cha
        [ValidateNever] public Product Product { get; set; } = null!;
        [ValidateNever] public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
        [ValidateNever] public ICollection<InvoiceDetail> InvoiceDetails { get; set; } = new List<InvoiceDetail>();
    }
}
