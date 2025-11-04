using System;
using System.Linq;
using System.Threading.Tasks;
using ASM_C_3.Data;
using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace ASM_C_3.Controllers
{
    [Authorize]
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IUserService _userService;
        private readonly IVariantService _variantService;
        private readonly ILogger<InvoiceController> _logger;
        private readonly UserManager<ApplicationUser> _userManager;

        public InvoiceController(
            IInvoiceService invoiceService,
            IUserService userService,
            IVariantService variantService,
            ILogger<InvoiceController> logger,
            UserManager<ApplicationUser> userManager)
        {
            _invoiceService = invoiceService;
            _userService = userService;
            _variantService = variantService;
            _logger = logger;
            _userManager = userManager;
        }

        // ✅ Admin/Staff thấy tất cả, User chỉ thấy hóa đơn của mình
        public async Task<IActionResult> Index()
        {
            var all = await _invoiceService.GetAllAsync();

            if (User.IsInRole("Admin") || User.IsInRole("Staff"))
            {
                ViewData["Title"] = "Manage Invoices";
                return View(all.OrderByDescending(i => i.CreatedDate));
            }

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser?.DomainUserId == null)
                return Forbid();

            var userInvoices = all
                .Where(i => i.UserId == appUser.DomainUserId.Value)
                .OrderByDescending(i => i.CreatedDate);

            ViewData["Title"] = "My Invoices";
            return View(userInvoices);
        }

        // ✅ Xem chi tiết
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            // User chỉ xem được đơn của mình
            if (!User.IsInRole("Admin") && !User.IsInRole("Staff"))
            {
                var appUser = await _userManager.GetUserAsync(User);
                if (appUser?.DomainUserId == null || invoice.UserId != appUser.DomainUserId.Value)
                    return Forbid();
            }

            return View(invoice);
        }

        // ✅ User gửi yêu cầu hủy đơn
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "User")]
        public async Task<IActionResult> RequestCancel(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser?.DomainUserId == null || invoice.UserId != appUser.DomainUserId.Value)
                return Forbid();

            if (invoice.Status is OrderStatus.Shipped or OrderStatus.Completed or OrderStatus.Cancelled or OrderStatus.CancelRequested)
            {
                TempData["Error"] = "Không thể yêu cầu hủy ở trạng thái hiện tại.";
                return RedirectToAction(nameof(Details), new { id });
            }

            invoice.Status = OrderStatus.CancelRequested;
            invoice.CancelReason ??= "Người dùng yêu cầu hủy đơn.";
            await _invoiceService.UpdateAsync(invoice);

            TempData["Success"] = "Đã gửi yêu cầu hủy đơn.";
            return RedirectToAction(nameof(My));
        }

        // ✅ Admin duyệt yêu cầu hủy
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ApproveCancel(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            if (invoice.Status != OrderStatus.CancelRequested)
            {
                TempData["Error"] = "Đơn hàng này không trong trạng thái yêu cầu hủy.";
                return RedirectToAction(nameof(Details), new { id });
            }

            invoice.Status = OrderStatus.Cancelled;
            await _invoiceService.UpdateAsync(invoice);

            TempData["Success"] = "Đã duyệt yêu cầu hủy đơn.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ✅ Admin từ chối yêu cầu hủy (trả về Pending)
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RejectCancel(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            if (invoice.Status != OrderStatus.CancelRequested)
            {
                TempData["Error"] = "Đơn hàng này không trong trạng thái yêu cầu hủy.";
                return RedirectToAction(nameof(Details), new { id });
            }

            invoice.Status = OrderStatus.Pending;
            await _invoiceService.UpdateAsync(invoice);

            TempData["Info"] = "Đã từ chối yêu cầu hủy. Đơn hàng quay lại trạng thái chờ duyệt.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ✅ Admin duyệt đơn hàng (Pending → Confirmed)
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmOrder(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            if (invoice.Status != OrderStatus.Pending)
            {
                TempData["Error"] = "Chỉ có thể duyệt đơn hàng đang ở trạng thái Pending.";
                return RedirectToAction(nameof(Details), new { id });
            }

            invoice.Status = OrderStatus.Confirmed;
            await _invoiceService.UpdateAsync(invoice);

            TempData["Success"] = "Đã duyệt đơn hàng thành công.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ✅ Admin đánh dấu là đã giao hàng
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> MarkAsShipped(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            if (invoice.Status != OrderStatus.Confirmed)
            {
                TempData["Error"] = "Chỉ có thể đánh dấu giao hàng khi đơn đã được duyệt.";
                return RedirectToAction(nameof(Details), new { id });
            }

            invoice.Status = OrderStatus.Shipped;
            await _invoiceService.UpdateAsync(invoice);

            TempData["Success"] = "Đơn hàng đã được đánh dấu là đã giao.";
            return RedirectToAction(nameof(Details), new { id });
        }

        // ✅ Admin / Staff hủy đơn hàng trực tiếp (Pending hoặc Confirmed)
        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CancelOrder(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            if (invoice.Status != OrderStatus.Pending && invoice.Status != OrderStatus.Confirmed)
            {
                TempData["Error"] = "Chỉ có thể hủy đơn hàng ở trạng thái Pending hoặc Confirmed.";
                return RedirectToAction(nameof(Index));
            }

            invoice.Status = OrderStatus.Cancelled;
            invoice.CancelReason ??= "Đơn hàng bị hủy bởi quản trị viên.";
            await _invoiceService.UpdateAsync(invoice);

            TempData["Success"] = "Đơn hàng đã được hủy thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ✅ User xác nhận đã nhận hàng
        [Authorize(Roles = "User")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmReceived(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null) return NotFound();

            var appUser = await _userManager.GetUserAsync(User);
            if (appUser?.DomainUserId == null || invoice.UserId != appUser.DomainUserId.Value)
                return Forbid();

            if (invoice.Status != OrderStatus.Shipped)
            {
                TempData["Error"] = "Chỉ có thể xác nhận khi đơn hàng đã được giao.";
                return RedirectToAction(nameof(Details), new { id });
            }

            invoice.Status = OrderStatus.Completed;
            await _invoiceService.UpdateAsync(invoice);

            TempData["Success"] = "Cảm ơn bạn! Đơn hàng đã được hoàn tất.";
            return RedirectToAction(nameof(My));
        }

        // ✅ My action: Xem hóa đơn của user
        public async Task<IActionResult> My()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null) return Challenge();
            if (appUser.DomainUserId == null) return RedirectToAction("Profile", "Account");

            var all = await _invoiceService.GetAllAsync();
            var myInvoices = all
                .Where(i => i.UserId == appUser.DomainUserId.Value)
                .OrderByDescending(i => i.CreatedDate);

            ViewData["Title"] = "My Invoices";
            return View("Index", myInvoices);
        }
    }
}
