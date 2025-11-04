using System.ComponentModel.DataAnnotations;

namespace ASM_C_3.ViewModels
{
    public class UserAddressViewModel
    {
        public int UserId { get; set; }

        //[Required(ErrorMessage = "Tên đăng nhập là bắt buộc")]
        //[StringLength(50)]
        //public string Username { get; set; } = string.Empty;

        [StringLength(100)]
        public string? FullName { get; set; }

        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        [StringLength(100)]
        public string? Email { get; set; }

        [Phone(ErrorMessage = "Số điện thoại không hợp lệ")]
        [StringLength(15)]
        public string? Phone { get; set; }

    }
}
