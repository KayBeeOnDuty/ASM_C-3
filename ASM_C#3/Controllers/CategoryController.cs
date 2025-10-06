using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace ASM_C_3.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly ILogger<CategoryController> _logger;

        public CategoryController(ICategoryService categoryService, ILogger<CategoryController> logger)
        {
            _categoryService = categoryService;
            _logger = logger;
        }

        // GET: Category
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllAsync();
            return View(categories);
        }

        // GET: Category/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {Id} not found", id);
                return NotFound();
            }
            return View(category);
        }

        // GET: Category/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Category/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Category category)
        {
            if (ModelState.IsValid)
            {
                await _categoryService.AddAsync(category);
                TempData["Success"] = "Categories added successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var kv in ModelState)
            {
                foreach (var err in kv.Value.Errors)
                {
                    _logger.LogWarning("Create Category ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
                }
            }

            return View(category);
        }

        // GET: Category/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {Id} not found for edit", id);
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Category category)
        {
            if (id != category.CategoryId)
            {
                _logger.LogWarning("Edit request ID {Id} does not match CategoryId {CategoryId}", id, category.CategoryId);
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _categoryService.UpdateAsync(category);
                TempData["Success"] = "Categories updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var kv in ModelState)
            {
                foreach (var err in kv.Value.Errors)
                {
                    _logger.LogWarning("Edit Category ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
                }
            }

            return View(category);
        }

        // GET: Category/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetByIdAsync(id);
            if (category == null)
            {
                _logger.LogWarning("Category with ID {Id} not found for delete", id);
                return NotFound();
            }
            return View(category);
        }

        // POST: Category/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _categoryService.DeleteAsync(id);
            TempData["Success"] = "Categories deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}

