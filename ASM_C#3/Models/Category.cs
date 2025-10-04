using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class Category
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CategoryId { get; set; } // Mã danh mục

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty; // Tên danh mục (VD: Milk Tea, Fruit Tea)

        public string? Description { get; set; } // Mô tả danh mục sản phẩm

        public ICollection<Product>? Products { get; set; } // Danh sách sản phẩm thuộc danh mục

    }
}
