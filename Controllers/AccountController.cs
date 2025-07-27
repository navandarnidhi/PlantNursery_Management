using Microsoft.AspNetCore.Mvc;
using PlantNurseryManagement.Models;
using Microsoft.EntityFrameworkCore;
using System.Security.Cryptography;
using System.Text;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PlantNurseryManagement.Controllers
{
    public class AccountController : Controller
    {
        private readonly MyNurseryDbContext _context;
        public AccountController(MyNurseryDbContext context)
        {
            _context = context;
        }

        // GET: /Account/Register
        public IActionResult Register()
        {
            return View();
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(RegisterViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Check if email already exists
                if (await _context.Users.AnyAsync(u => u.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(model);
                }

                // Check if admin email already exists
                if (await _context.Admins.AnyAsync(a => a.Email == model.Email))
                {
                    ModelState.AddModelError("Email", "Email already exists.");
                    return View(model);
                }

                if (model.UserType == "Admin")
                {
                    // Register as Admin
                    var admin = new Admin
                    {
                        Name = model.Name,
                        Email = model.Email,
                        PasswordHash = HashPassword(model.Password)
                    };
                    _context.Admins.Add(admin);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Admin account created successfully! Please login.";
                    return RedirectToAction("Login");
                }
                else
                {
                    // Register as User
                    var user = new User
                    {
                        Name = model.Name,
                        Email = model.Email,
                        PhoneNumber = model.PhoneNumber,
                        Address = model.Address,
                        City = model.City,
                        State = model.State,
                        PinCode = model.PinCode,
                        PasswordHash = HashPassword(model.Password)
                    };
                    _context.Users.Add(user);
                    await _context.SaveChangesAsync();
                    
                    TempData["Success"] = "Account created successfully! Please login.";
                    return RedirectToAction("Login");
                }
            }
            return View(model);
        }

        // GET: /Account/Login
        public IActionResult Login()
        {
            return View();
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel model)
        {
            if (ModelState.IsValid)
            {
                // Try to find user
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == model.Email);
                if (user != null && VerifyPassword(model.Password, user.PasswordHash))
                {
                    HttpContext.Session.SetString("UserEmail", user.Email);
                    HttpContext.Session.SetString("UserRole", "User");
                    return RedirectToAction("UserDashboard", "Account");
                }

                // Try to find admin
                var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == model.Email);
                if (admin != null && VerifyPassword(model.Password, admin.PasswordHash))
                {
                    HttpContext.Session.SetString("UserEmail", admin.Email);
                    HttpContext.Session.SetString("UserRole", "Admin");
                    return RedirectToAction("AdminDashboard", "Account");
                }

                ModelState.AddModelError(string.Empty, "Invalid login attempt.");
            }
            return View(model);
        }

        // GET: /Account/Logout
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("Login");
        }

        // GET: /Account/UserDashboard
        public IActionResult UserDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login");
            return View();
        }

        // GET: /Account/AdminDashboard
        public IActionResult AdminDashboard()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login");
            return View();
        }

        // Password hashing helpers
        private string HashPassword(string password)
        {
            using (var sha256 = SHA256.Create())
            {
                var bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
                var builder = new StringBuilder();
                foreach (var b in bytes)
                    builder.Append(b.ToString("x2"));
                return builder.ToString();
            }
        }
        private bool VerifyPassword(string password, string hash)
        {
            return HashPassword(password) == hash;
        }
    }
} 