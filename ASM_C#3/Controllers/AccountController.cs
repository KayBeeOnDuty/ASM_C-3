using ASM_C_3.Data;
using ASM_C_3.Models;
using ASM_C_3.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Linq;
using System.Threading.Tasks;

namespace ASM_C_3.Controllers
{
    public class AccountController : Controller
    {
        private readonly UserManager<ApplicationUser> _userManager;
        private readonly SignInManager<ApplicationUser> _signInManager;
        private readonly TraNgheDbContext _context;

        public AccountController(
            UserManager<ApplicationUser> userManager,
            SignInManager<ApplicationUser> signInManager,
            TraNgheDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
        }

        // ---------------- REGISTER ----------------
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Register()
        {
            if (User.Identity?.IsAuthenticated == true)
                return Forbid();
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(string username, string password)
        {
            if (User.Identity?.IsAuthenticated == true)
                return Forbid();

            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập và mật khẩu (tối thiểu 6 ký tự) là bắt buộc.");
                return View();
            }

            var user = new ApplicationUser { UserName = username, Email = username }; // set Email=username if you don't collect email separately
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                // create domain user and link
                var domainUser = new User
                {
                    FullName = username,
                    Email = user.Email,
                    Phone = null
                };
                _context.AppUsers.Add(domainUser);
                await _context.SaveChangesAsync();

                user.DomainUserId = domainUser.UserId;
                await _userManager.UpdateAsync(user);

                await _userManager.AddToRoleAsync(user, "User");
                await _signInManager.SignInAsync(user, isPersistent: false);
                return RedirectToAction("Index", "Home");
            }

            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);

            return View();
        }

        // ---------------- LOGIN ----------------
        [HttpGet]
        [AllowAnonymous]
        public IActionResult Login(string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            return View();
        }

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(string username, string password, string? returnUrl = null)
        {
            ViewData["ReturnUrl"] = returnUrl;
            var result = await _signInManager.PasswordSignInAsync(username, password, isPersistent: false, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                if (!string.IsNullOrEmpty(returnUrl) && Url.IsLocalUrl(returnUrl))
                    return Redirect(returnUrl);
                return RedirectToAction("Index", "Home");
            }

            ModelState.AddModelError(string.Empty, "Đăng nhập không hợp lệ.");
            return View();
        }

        // ---------------- LOGOUT ----------------
        [Authorize]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            return RedirectToAction("Index", "Home");
        }

        // ---------------- ACCESS DENIED ----------------
        [HttpGet]
        [AllowAnonymous]
        public IActionResult AccessDenied() => View();

        // ---------------- CREATE STAFF ----------------
        [HttpGet]
        [Authorize(Roles = "Admin")]
        public IActionResult CreateStaff() => View();

        [HttpPost]
        [Authorize(Roles = "Admin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateStaff(string username, string password)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password) || password.Length < 6)
            {
                ModelState.AddModelError(string.Empty, "Tên đăng nhập và mật khẩu (tối thiểu 6 ký tự) là bắt buộc.");
                return View();
            }

            var user = new ApplicationUser { UserName = username };
            var result = await _userManager.CreateAsync(user, password);

            if (result.Succeeded)
            {
                await _userManager.AddToRoleAsync(user, "Staff");
                TempData["Success"] = "Tạo tài khoản nhân viên thành công.";
                return RedirectToAction("Index", "Home");
            }

            foreach (var err in result.Errors)
                ModelState.AddModelError(string.Empty, err.Description);

            return View();
        }

        // ---------------- PROFILE ----------------
        [HttpGet]
        [Authorize]
        public async Task<IActionResult> Profile()
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
                return RedirectToAction("Login");

            var vm = new UserAddressViewModel();

            if (appUser.DomainUserId is int id && id > 0)
            {
                var domainUser = await _context.AppUsers.FindAsync(id);
                if (domainUser != null)
                {
                    vm = new UserAddressViewModel
                    {
                        UserId = domainUser.UserId,
                        FullName = domainUser.FullName,
                        Email = domainUser.Email,
                        Phone = domainUser.Phone
                    };
                }
            }

            return View(vm);
        }

        [HttpPost]
        [Authorize]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile(UserAddressViewModel model)
        {
            var appUser = await _userManager.GetUserAsync(User);
            if (appUser == null)
                return RedirectToAction("Login");

            if (!ModelState.IsValid)
                return View(model);

            User domainUser;
            if (appUser.DomainUserId is int id && id > 0)
            {
                domainUser = await _context.AppUsers.FindAsync(id) ?? new User();
                if (domainUser.UserId == 0)
                    _context.AppUsers.Add(domainUser);
            }
            else
            {
                domainUser = new User();
                _context.AppUsers.Add(domainUser);
            }

            domainUser.FullName = model.FullName;
            domainUser.Email = model.Email;
            domainUser.Phone = model.Phone;

            await _context.SaveChangesAsync();

            if (!(appUser.DomainUserId is int id2) || id2 <= 0)
            {
                appUser.DomainUserId = domainUser.UserId;
                await _userManager.UpdateAsync(appUser);
            }

            TempData["Success"] = "Cập nhật hồ sơ thành công.";
            return RedirectToAction("Profile");
        }
    }
}
