using ASM_C_3.Data;
using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_C_3.Controllers
{
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartService _cartService;
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly TraNgheDbContext _context;
        private readonly ILogger<CartController> _logger;

        public CartController(
            ICartService cartService,
            UserManager<ApplicationUser> userManager,
            TraNgheDbContext context,
            ILogger<CartController> logger)
        {
            _cartService = cartService;
            _userManager = userManager;
            _context = context;
            _logger = logger;
        }

        // ---------------- Helper ----------------
        private async Task<int?> GetDomainUserIdAsync()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser?.DomainUserId is int id && id > 0)
                return id;

            TempData["ErrorMessage"] = " Vui lòng cập nhật hồ sơ trước khi đặt hàng.";
            return null;
        }

        // ---------------- USER: Xem giỏ hàng cá nhân ----------------
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Index()
        {
            var userId = await GetDomainUserIdAsync();
            if (!userId.HasValue)
                return RedirectToAction("Profile", "Account");

            var cart = await _cartService.GetCartWithItemsAsync(userId.Value);
            return View(cart);
        }

        // ---------------- ADMIN/STAFF: Quản lý tất cả giỏ hàng ----------------
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Manage()
        {
            var carts = await _context.Carts
                .Include(c => c.User)
                .OrderByDescending(c => c.CreatedDate)
                .ToListAsync();

            return View(carts);
        }

        // ---------------- ADD TO CART (USER) ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> AddToCart(int variantId, int quantity = 1)
        {
            var userId = await GetDomainUserIdAsync();
            if (!userId.HasValue)
                return RedirectToAction("Profile", "Account");

            var variant = await _context.Variants.FindAsync(variantId);
            if (variant == null || !variant.IsAvailable)
            {
                TempData["ErrorMessage"] = "Sản phẩm không tồn tại hoặc đã ngừng bán.";
                return RedirectToAction("Index", "Product");
            }

            await _cartService.AddItemAsync(userId.Value, variantId, quantity);
            TempData["Success"] = $" Đã thêm {variant.Name} vào giỏ hàng!";
            return RedirectToAction("Index");
        }

        // ---------------- REMOVE FROM CART (USER) ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RemoveFromCart(int cartDetailId)
        {
            var userId = await GetDomainUserIdAsync();
            if (!userId.HasValue)
                return RedirectToAction("Profile", "Account");

            var detail = await _context.CartDetails
                .Include(cd => cd.Cart)
                .FirstOrDefaultAsync(cd => cd.CartDetailId == cartDetailId);

            if (detail == null || detail.Cart.UserId != userId.Value)
            {
                TempData["ErrorMessage"] = " Không tìm thấy sản phẩm trong giỏ hàng của bạn.";
                return RedirectToAction(nameof(Index));
            }

            await _cartService.RemoveItemAsync(cartDetailId);
            TempData["Success"] = " Đã xóa sản phẩm khỏi giỏ hàng!";
            return RedirectToAction(nameof(Index));
        }

        // ---------------- CLEAR CART (USER) ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> ClearCart()
        {
            var userId = await GetDomainUserIdAsync();
            if (!userId.HasValue)
                return RedirectToAction("Profile", "Account");

            await _cartService.ClearCartAsync(userId.Value);
            TempData["Success"] = " Giỏ hàng đã được làm trống.";
            return RedirectToAction("Index");
        }

        // ---------------- CHECKOUT (USER) ----------------
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> Checkout(PayMethod payMethod = PayMethod.Cash)
        {
            var userId = await GetDomainUserIdAsync();
            if (!userId.HasValue)
                return RedirectToAction("Profile", "Account");

            try
            {
                var cart = await _cartService.GetCartWithItemsAsync(userId.Value);
                if (cart == null || !cart.CartDetails.Any())
                {
                    TempData["ErrorMessage"] = " Giỏ hàng của bạn đang trống.";
                    return RedirectToAction("Index");
                }

                var invoice = await _cartService.CheckoutAsync(userId.Value, payMethod);
                TempData["Success"] = " Đặt hàng thành công! Đơn hàng của bạn đang chờ xử lý.";
                return RedirectToAction("Details", "Invoice", new { id = invoice.InvoiceId });
            }
            catch (InvalidOperationException ex)
            {
                _logger.LogError(ex, "Checkout failed for user {UserId}", userId);
                TempData["ErrorMessage"] = ex.Message;
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error during checkout for user {UserId}", userId);
                TempData["ErrorMessage"] = "Đã xảy ra lỗi không mong muốn. Vui lòng thử lại.";
                return RedirectToAction("Index");
            }
        }
    }
}
