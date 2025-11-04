using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; } // Mã sản phẩm
        [Range(1, int.MaxValue, ErrorMessage = "Select a category")]
        public int CategoryId { get; set; } // Mã danh mục
        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty; // Tên sản phẩm (VD: Brown Sugar Milk Tea)
        [Range(0, int.MaxValue, ErrorMessage = "Price must be >= 0")]
        public int Price { get; set; }
        [Required, StringLength(500)]
        public string? Description { get; set; } = string.Empty; // Mô tả sản phẩm


        public string? ImageUrl { get; set; } // Hình ảnh sản phẩm

        public int SupplierId { get; set; } // Mã nhà cung cấp


        [ValidateNever] public Category Category { get; set; } = null!;
        [ValidateNever] public Supplier? Supplier { get; set; }
        [ValidateNever] public ICollection<Variant> Variants { get; set; } = new List<Variant>();// Các biến thể (size, topping,…)
    }
}
