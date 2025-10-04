using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class Supplier
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int SupplierId { get; set; } // Mã nhà cung cấp

        [Required, StringLength(100)]
        public string Name { get; set; } = string.Empty; // Tên nhà cung cấp

        [StringLength(150)]
        public string? ContactInfo { get; set; } // Thông tin liên hệ (email/sđt)

        [StringLength(200)]
        public string? Address { get; set; } // Địa chỉ nhà cung cấp

        public ICollection<Product>? Products { get; set; } // Danh sách sản phẩm cung cấp
    }
}
