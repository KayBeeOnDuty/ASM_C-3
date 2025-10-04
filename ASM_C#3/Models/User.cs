using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class User
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int UserId { get; set; } // Mã người dùng

        [Required, StringLength(50)]
        public string Username { get; set; } = string.Empty; // Tên đăng nhập

        [Required, StringLength(100)]
        public string PasswordHash { get; set; } = string.Empty; // Mật khẩu (đã mã hóa)

        [StringLength(100)]
        public string? FullName { get; set; } // Họ tên người dùng

        [StringLength(100)]
        public string? Email { get; set; } // Email người dùng

        [StringLength(15)]
        public string? Phone { get; set; } // Số điện thoại

        public int RoleId { get; set; } // Mã vai trò
        public Role? Role { get; set; }

        public ICollection<Cart>? Carts { get; set; } // Giỏ hàng của người dùng
        public ICollection<Invoice>? Invoices { get; set; } // Hóa đơn của người dùng
    }
}
