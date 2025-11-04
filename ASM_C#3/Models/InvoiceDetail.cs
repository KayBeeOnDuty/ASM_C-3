using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class InvoiceDetail
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int InvoiceDetailId { get; set; } // Mã dòng chi tiết hóa đơn
        [Range(1, int.MaxValue, ErrorMessage = "Select an invoice")]
        public int InvoiceId { get; set; }
        [Range(1, int.MaxValue, ErrorMessage = "Select a variant")]
        public int VariantId { get; set; }

        [Range(1, int.MaxValue)] public int Quantity { get; set; } = 1;
        [Range(0, int.MaxValue)] public int UnitPrice { get; set; }
        [Range(0, int.MaxValue)] public int SubTotal { get; set; }
        [ValidateNever] public Invoice Invoice { get; set; } = null!;
        [ValidateNever] public Variant Variant { get; set; } = null; // Biến thể được bán
    }
}
