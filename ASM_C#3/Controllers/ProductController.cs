using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace ASM_C_3.Controllers
{
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

        // GET: Product
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllAsync();
            return View(products);
        }

        // GET: Product/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // GET: Product/Create
        public IActionResult Create()
        {
            PopulateDropdowns();
            return View();
        }

        // POST: Product/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Product product, IFormFile? ImageFile)
        {
            if (ModelState.IsValid)
            {
                // 🔹 Nếu người dùng có chọn file ảnh
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
                TempData["Success"] = "Product added successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var kv in ModelState)
            {
                foreach (var err in kv.Value.Errors)
                {
                    _logger.LogWarning("Create Product ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
                }
            }

            PopulateDropdowns(product.CategoryId, product.SupplierId);
            return View(product);
        }

        // GET: Product/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();

            PopulateDropdowns(product.CategoryId, product.SupplierId);
            return View(product);
        }

        // POST: Product/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Product product, IFormFile? ImageFile)
        {
            if (id != product.ProductId) return BadRequest();

            if (ModelState.IsValid)
            {
                // 🔹 Nếu người dùng upload ảnh mới
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
                    // 🔹 Giữ nguyên ảnh cũ nếu không upload mới
                    var existingProduct = await _productService.GetByIdAsync(product.ProductId);
                    product.ImageUrl = existingProduct?.ImageUrl;
                }

                await _productService.UpdateAsync(product);
                TempData["Success"] = "Product updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var kv in ModelState)
            {
                foreach (var err in kv.Value.Errors)
                {
                    _logger.LogWarning("Edit Product ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
                }
            }

            PopulateDropdowns(product.CategoryId, product.SupplierId);
            return View(product);
        }

        // GET: Product/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetByIdAsync(id);
            if (product == null) return NotFound();
            return View(product);
        }

        // POST: Product/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _productService.DeleteAsync(id);
            TempData["Success"] = "Product deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
