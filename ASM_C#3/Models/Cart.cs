using Microsoft.AspNetCore.Mvc.ModelBinding.Validation;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ASM_C_3.Models
{
    public class Cart
    {
        [Key, DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartId { get; set; } // Mã giỏ hàng

        [Required] public int UserId { get; set; } // Người sở hữu giỏ hàng

        public DateTime CreatedDate { get; set; } = DateTime.UtcNow; // Ngày tạo giỏ hàng

        [Range(0, int.MaxValue)]
        public int TotalAmount { get; set; }

        public bool Status { get; set; } = true;
        [ValidateNever] public User User { get; set; } = null!;
        [ValidateNever] public ICollection<CartDetail> CartDetails { get; set; } = new List<CartDetail>();
    }
}
