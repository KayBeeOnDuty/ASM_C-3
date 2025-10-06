using ASM_C_3.Interface;
using ASM_C_3.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Threading.Tasks;

namespace BanhMyIT.Controllers
{
    public class InvoiceController : Controller
    {
        private readonly IInvoiceService _invoiceService;
        private readonly IUserService _userService;
        private readonly IVariantService _variantService;
        private readonly ILogger<InvoiceController> _logger;

        public InvoiceController(
            IInvoiceService invoiceService,
            IUserService userService,
            IVariantService variantService,
            ILogger<InvoiceController> logger)
        {
            _invoiceService = invoiceService;
            _userService = userService;
            _variantService = variantService;
            _logger = logger;
        }

        // GET: Invoice
        public async Task<IActionResult> Index()
        {
            var invoices = await _invoiceService.GetAllAsync();
            return View(invoices);
        }

        // GET: Invoice/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null)
            {
                _logger.LogWarning("Invoice with ID {Id} not found", id);
                return NotFound();
            }
            return View(invoice);
        }

        // GET: Invoice/Create
        public async Task<IActionResult> Create()
        {
            ViewBag.Users = await _userService.GetAllAsync();
            ViewBag.Variants = await _variantService.GetAllAsync();
            return View();
        }

        // POST: Invoice/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Invoice invoice)
        {
            if (ModelState.IsValid)
            {
                invoice.CreatedDate = DateTime.Now;
                await _invoiceService.AddAsync(invoice);
                TempData["Success"] = "Invoices created successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var kv in ModelState)
            {
                foreach (var err in kv.Value.Errors)
                {
                    _logger.LogWarning("Create Invoice ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
                }
            }

            ViewBag.Users = await _userService.GetAllAsync();
            ViewBag.Variants = await _variantService.GetAllAsync();
            return View(invoice);
        }

        // GET: Invoice/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null)
            {
                _logger.LogWarning("Invoice with ID {Id} not found for edit", id);
                return NotFound();
            }

            ViewBag.Users = await _userService.GetAllAsync();
            return View(invoice);
        }

        // POST: Invoice/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Invoice invoice)
        {
            if (id != invoice.InvoiceId)
            {
                _logger.LogWarning("Edit request ID {Id} does not match InvoiceId {InvoiceId}", id, invoice.InvoiceId);
                return BadRequest();
            }

            if (ModelState.IsValid)
            {
                await _invoiceService.UpdateAsync(invoice);
                TempData["Success"] = "Invoice updated successfully!";
                return RedirectToAction(nameof(Index));
            }

            foreach (var kv in ModelState)
            {
                foreach (var err in kv.Value.Errors)
                {
                    _logger.LogWarning("Edit Invoice ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);
                }
            }

            ViewBag.Users = await _userService.GetAllAsync();
            return View(invoice);
        }

        // GET: Invoice/Delete/5
        public async Task<IActionResult> Delete(int id)
        {
            var invoice = await _invoiceService.GetByIdAsync(id);
            if (invoice == null)
            {
                _logger.LogWarning("Invoice with ID {Id} not found for delete", id);
                return NotFound();
            }
            return View(invoice);
        }

        // POST: Invoice/DeleteConfirmed/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _invoiceService.DeleteAsync(id);
            TempData["Success"] = "Invoice deleted successfully!";
            return RedirectToAction(nameof(Index));
        }
    }
}
