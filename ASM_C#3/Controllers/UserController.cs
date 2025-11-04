using ASM_C_3.Data;
using ASM_C_3.Interface;
using ASM_C_3.Models;
using ASM_C_3.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_C_3.Controllers
{
    [Authorize(Roles = "Admin,Staff")]
    public class UserController : Controller
    {
        private readonly IUserService _userService;
        private readonly ILogger<UserController> _logger;
        private readonly TraNgheDbContext _db;

        public UserController(IUserService userService, ILogger<UserController> logger, TraNgheDbContext db)
        {
            _userService = userService;
            _logger = logger;
            _db = db;
        }

        // ---------------- INDEX ----------------
        public async Task<IActionResult> Index()
        {
            var users = await _db.AppUsers.ToListAsync();
            return View(users);
        }

        // ---------------- DETAILS ----------------
        public async Task<IActionResult> Details(int id)
        {
            var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        // ---------------- CREATE ----------------
        [HttpGet]
        public IActionResult Create()
        {
            return View(new UserAddressViewModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Phone = model.Phone
                };

                await _userService.AddAsync(user);
                TempData["Success"] = "Tạo người dùng thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Ghi log lỗi nếu có
            foreach (var kv in ModelState)
                foreach (var err in kv.Value.Errors)
                    _logger.LogWarning("Create User ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);

            return View(model);
        }

        // ---------------- EDIT ----------------
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                return NotFound();

            var vm = new UserAddressViewModel
            {
                UserId = user.UserId,
                FullName = user.FullName,
                Email = user.Email,
                Phone = user.Phone
            };

            return View(vm);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(UserAddressViewModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _db.AppUsers.FindAsync(model.UserId);
                if (user == null)
                    return NotFound();

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.Phone = model.Phone;

                await _userService.UpdateAsync( user);
                TempData["Success"] = "Cập nhật người dùng thành công!";
                return RedirectToAction(nameof(Index));
            }

            // Log lỗi
            foreach (var kv in ModelState)
                foreach (var err in kv.Value.Errors)
                    _logger.LogWarning("Edit User ModelState error for {Field}: {Error}", kv.Key, err.ErrorMessage);

            return View(model);
        }

        // ---------------- DELETE ----------------
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var user = await _db.AppUsers.FirstOrDefaultAsync(u => u.UserId == id);
            if (user == null)
                return NotFound();

            return View(user);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _userService.DeleteAsync(id);
            TempData["Success"] = "Xóa người dùng thành công!";
            return RedirectToAction(nameof(Index));
        }
    }
}
