using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class Role
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int RoleId { get; set; } // Mã vai trò

        [Required, StringLength(50)]
        public string Name { get; set; } = string.Empty; // Tên vai trò (Admin, Staff, Customer)

        public ICollection<User>? Users { get; set; } // Người dùng có vai trò này
    }
}
