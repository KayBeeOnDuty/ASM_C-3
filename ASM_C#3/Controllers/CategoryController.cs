using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ASM_C_3.Controllers
{
    [Authorize] // Tất cả hành động đều yêu cầu đăng nhập
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        // ---------------- DANH SÁCH ----------------
        [AllowAnonymous] // Ai cũng có thể xem danh sách
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        // ---------------- CHI TIẾT ----------------
        [AllowAnonymous] // Ai cũng có thể xem chi tiết
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Không tìm thấy danh mục có ID {Id}", id);
                return NotFound();
            }
            return View(category);
        }

        // ---------------- THÊM MỚI ----------------
        [Authorize(Roles = "Admin,Staff")]
        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddAsync(category);
                TempData["Success"] = " Thêm danh mục mới thành công!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var kv in ModelState)
            {
                foreach (var err in kv.Value.Errors)
                {
                    _logger.LogWarning("Lỗi khi thêm danh mục (trường {Field}): {Error}", kv.Key, err.ErrorMessage);
                }
            }

            TempData["Error"] = " Không thể thêm danh mục. Vui lòng kiểm tra lại thông tin.";
            return View(category);
        }

        // ---------------- CHỈNH SỬA ----------------
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Không tìm thấy danh mục có ID {Id} để chỉnh sửa", id);
                return NotFound();
            }
            return View(category);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                _logger.LogWarning("ID trong yêu cầu ({Id}) không khớp với CategoryId ({CategoryId})", id, category.CategoryId);
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _categoryService.UpdateAsync(category);
                TempData["Success"] = "✏️ Cập nhật danh mục thành công!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var kv in ModelState)
            {
                foreach (var err in kv.Value.Errors)
                {
                    _logger.LogWarning("Lỗi khi cập nhật danh mục (trường {Field}): {Error}", kv.Key, err.ErrorMessage);
                }
            }

            TempData["Error"] = " Không thể cập nhật danh mục. Vui lòng thử lại.";
            return View(category);
        }

        // ---------------- XÓA ----------------
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Không tìm thấy danh mục có ID {Id} để xóa", id);
                return NotFound();
            }
            return View(category);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryService.DeleteAsync(id);
            TempData["Success"] = " Xóa danh mục thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
