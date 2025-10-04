using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class Cart
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; } // Mã giỏ hàng

        public int UserId { get; set; } // Người sở hữu giỏ hàng
        public User? User { get; set; }

        public DateTime CreatedDate { get; set; } = DateTime.Now; // Ngày tạo giỏ hàng

        public ICollection<CartDetail>? CartDetails { get; set; } // Danh sách sản phẩm trong giỏ
    }
}
