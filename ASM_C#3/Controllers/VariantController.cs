using ASM_C_3.Data;
using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_C_3.Controllers
{
    public class VariantController : Controller
    {
        private readonly IVariantService _variantService;
        private readonly ILogger<VariantController> _logger;
        private readonly TraNgheDbContext _context;

        public VariantController(IVariantService variantService, ILogger<VariantController> logger, TraNgheDbContext context)
        {
            _variantService = variantService;
            _logger = logger;
            _context = context;
        }

        // ---------------- DANH SÁCH ----------------
        [AllowAnonymous] // Ai cũng có thể xem danh sách
        public async Task<IActionResult> Index()
        {
            var variants = await _variantService.GetAllAsync();
            return View(variants);
        }

        // ---------------- CHI TIẾT ----------------
        [AllowAnonymous] // Ai cũng có thể xem chi tiết
        public async Task<IActionResult> Details(int id)
        {
            var variant = await _variantService.GetByIdAsync(id);
            if (variant == null)
            {
                _logger.LogWarning("Không tìm thấy biến thể có ID {Id}", id);
                return NotFound();
            }
            return View(variant);
        }

        // ---------------- THÊM MỚI ----------------
        [Authorize(Roles = "Admin,Staff")] // Chỉ Admin hoặc Nhân viên có thể tạo
        public IActionResult Create()
        {
            var products = _context.Products
                .Select(p => new { p.ProductId, p.Name })
                .ToList();

            if (!products.Any())
                TempData["Warning"] = "Không có sản phẩm nào. Vui lòng tạo sản phẩm trước.";

            ViewData["ProductId"] = new SelectList(products, "ProductId", "Name");
            return View();
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Variant variant)
        {
            if (ModelState.IsValid)
            {
                await _variantService.AddAsync(variant);
                TempData["Success"] = "Thêm biến thể sản phẩm thành công.";
                return RedirectToAction(nameof(Index));
            }

            var products = _context.Products
                .Select(p => new { p.ProductId, p.Name })
                .ToList();

            ViewData["ProductId"] = new SelectList(products, "ProductId", "Name", variant.ProductId);
            return View(variant);
        }

        // ---------------- CHỈNH SỬA ----------------
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id)
        {
            var variant = await _variantService.GetByIdAsync(id);
            if (variant == null)
            {
                _logger.LogWarning("Không tìm thấy biến thể có ID {Id} để chỉnh sửa", id);
                return NotFound();
            }

            var products = _context.Products
                .Select(p => new { p.ProductId, p.Name })
                .ToList();

            ViewData["ProductId"] = new SelectList(products, "ProductId", "Name", variant.ProductId);
            return View(variant);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Variant variant)
        {
            if (id != variant.VariantId)
            {
                TempData["Error"] = "Mã biến thể không hợp lệ.";
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _variantService.UpdateAsync(variant);
                TempData["Success"] = "Cập nhật biến thể sản phẩm thành công.";
                return RedirectToAction(nameof(Index));
            }

            var products = _context.Products
                .Select(p => new { p.ProductId, p.Name })
                .ToList();

            ViewData["ProductId"] = new SelectList(products, "ProductId", "Name", variant.ProductId);
            return View(variant);
        }

        // ---------------- XÓA ----------------
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var variant = await _variantService.GetByIdAsync(id);
            if (variant == null)
            {
                _logger.LogWarning("Không tìm thấy biến thể có ID {Id} để xóa", id);
                return NotFound();
            }
            return View(variant);
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var existing = await _variantService.GetByIdAsync(id);
            if (existing == null)
            {
                TempData["Error"] = "Không tìm thấy biến thể để xóa.";
                return RedirectToAction(nameof(Index));
            }

            await _variantService.DeleteAsync(id);
            TempData["Success"] = "Xóa biến thể sản phẩm thành công.";
            return RedirectToAction(nameof(Index));
        }

        // ---------------- API: Lấy danh sách biến thể theo sản phẩm ----------------
        [AllowAnonymous]
        [HttpGet]
        public async Task<IActionResult> GetByProduct(int productId)
        {
            var variants = await _context.Variants
                .Where(v => v.ProductId == productId && v.IsAvailable)
                .Select(v => new { v.VariantId, v.Name, v.Price })
                .ToListAsync();

            if (!variants.Any())
            {
                return Json(new { message = "Không tìm thấy biến thể nào cho sản phẩm này." });
            }

            return Json(variants);
        }
    }
}
