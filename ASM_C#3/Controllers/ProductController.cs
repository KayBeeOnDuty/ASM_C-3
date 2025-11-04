using ASM_C_3.Data;
using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASM_C_3.Controllers
{
    [Authorize] // Yêu cầu đăng nhập để truy cập Product Controller
    public class ProductController : Controller
    {
        private readonly IProductService _productService;
        private readonly TraNgheDbContext _context;
        private readonly ILogger<ProductController> _logger;
        private readonly IWebHostEnvironment _env;

        public ProductController(
            IProductService productService,
            TraNgheDbContext context,
            ILogger<ProductController> logger,
            IWebHostEnvironment env)
        {
            _productService = productService;
            _context = context;
            _logger = logger;
            _env = env;
        }

        private void PopulateDropdowns(int? selectedCategoryId = null, int? selectedSupplierId = null)
        {
            ViewBag.Categories = new SelectList(_context.Categories.ToList(), "CategoryId", "Name", selectedCategoryId);
            ViewBag.Suppliers = new SelectList(_context.Suppliers.ToList(), "SupplierId", "Name", selectedSupplierId);
        }

        // -------------------- INDEX --------------------
        [AllowAnonymous] // Cho phép bất kỳ ai (kể cả chưa đăng nhập) xem sản phẩm
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }

        // -------------------- DETAILS --------------------
        [AllowAnonymous]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // -------------------- CREATE --------------------
        [Authorize(Roles = "Admin,Staff")] // Chỉ Admin và Staff có thể tạo
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/images/" + uniqueFileName;
                }

                await _productService.AddAsync(product);
                TempData["Success"] = "Sản phẩm đã được thêm thành công!";
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(product.CategoryId, product.SupplierId);
            return View(product);
        }

        // -------------------- EDIT --------------------
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            PopulateDropdowns(product.CategoryId, product.SupplierId);
            return View(product);
        }

        [HttpPost]
        [Authorize(Roles = "Admin,Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? ImageFile)
        {
            if (id != product.ProductId) return BadRequest();

            if (ModelState.IsValid)
            {
                if (ImageFile != null && ImageFile.Length > 0)
                {
                    string uploadsFolder = Path.Combine(_env.WebRootPath, "images");
                    Directory.CreateDirectory(uploadsFolder);

                    string uniqueFileName = Guid.NewGuid().ToString() + "_" + Path.GetFileName(ImageFile.FileName);
                    string filePath = Path.Combine(uploadsFolder, uniqueFileName);

                    using (var fileStream = new FileStream(filePath, FileMode.Create))
                    {
                        await ImageFile.CopyToAsync(fileStream);
                    }

                    product.ImageUrl = "/images/" + uniqueFileName;
                }
                else
                {
                    var existingProduct = await _productService.GetByIdAsync(product.ProductId);
                    product.ImageUrl = existingProduct?.ImageUrl;
                }

                await _productService.UpdateAsync(product);
                TempData["Success"] = "Sản phẩm đã được cập nhật thành công!";
                return RedirectToAction(nameof(Index));
            }

            PopulateDropdowns(product.CategoryId, product.SupplierId);
            return View(product);
        }

        // -------------------- DELETE --------------------
        [Authorize(Roles = "Admin,Staff")]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        [HttpPost, ActionName("Delete")]
        [Authorize(Roles = "Admin,Staff")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteAsync(id);
            TempData["Success"] = "Sản phẩm đã được xóa thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
