using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class Product
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ProductId { get; set; } // Mã sản phẩm

        [Required, StringLength(150)]
        public string Name { get; set; } = string.Empty; // Tên sản phẩm (VD: Brown Sugar Milk Tea)

        public string? Description { get; set; } // Mô tả sản phẩm

        [StringLength(200)]
        public string? ImageUrl { get; set; } // Hình ảnh sản phẩm

        public int SupplierId { get; set; } // Mã nhà cung cấp
        public Supplier? Supplier { get; set; }

        public int CategoryId { get; set; } // Mã danh mục
        public Category? Category { get; set; }

        public ICollection<Variant>? Variants { get; set; } // Các biến thể (size, topping,…)
    }
}
