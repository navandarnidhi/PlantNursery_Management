using Microsoft.AspNetCore.Mvc;
using PlantNurseryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace PlantNurseryManagement.Controllers
{
    public class CartController : Controller
    {
        private readonly MyNurseryDbContext _context;
        
        public CartController(MyNurseryDbContext context)
        {
            _context = context;
        }

        private int? GetCurrentUserId()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            return user?.UserId;
        }

        // GET: Cart/Index
        public async Task<IActionResult> Index()
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
                
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var cartItems = await _context.CartItems
                .Include(c => c.Plant)
                .Where(c => c.UserId == userId)
                .ToListAsync();
                
            return View(cartItems);
        }

        // POST: Cart/AddToCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddToCart(int plantId, int quantity = 1)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
                
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var plant = await _context.Plants.FindAsync(plantId);
            if (plant == null) return NotFound();
            
            if (quantity < 1 || quantity > plant.QuantityAvailable)
            {
                TempData["Error"] = $"Quantity must be between 1 and {plant.QuantityAvailable}.";
                return RedirectToAction("UserList", "Plants");
            }
            
            // Check if item already exists in cart
            var existingCartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.UserId == userId && c.PlantId == plantId);
                
            if (existingCartItem != null)
            {
                existingCartItem.Quantity += quantity;
                if (existingCartItem.Quantity > plant.QuantityAvailable)
                {
                    TempData["Error"] = $"Total quantity cannot exceed {plant.QuantityAvailable}.";
                    return RedirectToAction("UserList", "Plants");
                }
            }
            else
            {
                var cartItem = new CartItem
                {
                    UserId = userId.Value,
                    PlantId = plantId,
                    Quantity = quantity,
                    AddedDate = DateTime.Now
                };
                _context.CartItems.Add(cartItem);
            }
            
            await _context.SaveChangesAsync();
            TempData["Success"] = $"{plant.Name} added to cart!";
            return RedirectToAction("UserList", "Plants");
        }

        // POST: Cart/UpdateQuantity
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateQuantity(int cartItemId, int quantity)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
                
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var cartItem = await _context.CartItems
                .Include(c => c.Plant)
                .FirstOrDefaultAsync(c => c.CartItemId == cartItemId && c.UserId == userId);
                
            if (cartItem == null) return NotFound();
            
            if (quantity < 1 || quantity > cartItem.Plant.QuantityAvailable)
            {
                TempData["Error"] = $"Quantity must be between 1 and {cartItem.Plant.QuantityAvailable}.";
                return RedirectToAction("Index");
            }
            
            cartItem.Quantity = quantity;
            await _context.SaveChangesAsync();
            
            return RedirectToAction("Index");
        }

        // POST: Cart/RemoveFromCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RemoveFromCart(int cartItemId)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
                
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var cartItem = await _context.CartItems
                .FirstOrDefaultAsync(c => c.CartItemId == cartItemId && c.UserId == userId);
                
            if (cartItem == null) return NotFound();
            
            _context.CartItems.Remove(cartItem);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Item removed from cart!";
            return RedirectToAction("Index");
        }

        // POST: Cart/ClearCart
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ClearCart()
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
                
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var cartItems = await _context.CartItems
                .Where(c => c.UserId == userId)
                .ToListAsync();
                
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            
            TempData["Success"] = "Cart cleared!";
            return RedirectToAction("Index");
        }
    }
} 