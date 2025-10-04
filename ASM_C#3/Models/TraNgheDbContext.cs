using Microsoft.EntityFrameworkCore;

namespace ASM_C_3.Models
{
    public class TraNgheDbContext : DbContext
    {
        public TraNgheDbContext(DbContextOptions<TraNgheDbContext> options) : base(options)
        {
        }
        public DbSet<Supplier> Suppliers { get; set; } // Nhà cung cấp
        public DbSet<Category> Categories { get; set; } // Danh mục sản phẩm
        public DbSet<Product> Products { get; set; } // Sản phẩm
        public DbSet<Variant> Variants { get; set; } // Biến thể (size, topping,...)
        public DbSet<User> Users { get; set; } // Người dùng
        public DbSet<Role> Roles { get; set; } // Vai trò người dùng
        public DbSet<Cart> Carts { get; set; } // Giỏ hàng
        public DbSet<CartDetail> CartDetails { get; set; } // Chi tiết giỏ hàng
        public DbSet<Invoice> Invoices { get; set; } // Hóa đơn
        public DbSet<InvoiceDetail> InvoiceDetails { get; set; } // Chi tiết hóa đơn

    }
}
