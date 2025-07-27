using Microsoft.AspNetCore.Mvc;
using PlantNurseryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace PlantNurseryManagement.Controllers
{
    public class OrderController : Controller
    {
        private readonly MyNurseryDbContext _context;
        
        public OrderController(MyNurseryDbContext context)
        {
            _context = context;
        }

        private int? GetCurrentUserId()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            return user?.UserId;
        }

        // GET: Order/Checkout
        public async Task<IActionResult> Checkout()
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
                
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var cartItems = await _context.CartItems
                .Include(c => c.Plant)
                .Where(c => c.UserId == userId)
                .ToListAsync();
                
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart");
            }
            
            return View(cartItems);
        }

        // POST: Order/ProcessOrder
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ProcessOrder(string shippingAddress, string phoneNumber, string paymentMethod)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
                
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var cartItems = await _context.CartItems
                .Include(c => c.Plant)
                .Where(c => c.UserId == userId)
                .ToListAsync();
                
            if (!cartItems.Any())
            {
                TempData["Error"] = "Your cart is empty!";
                return RedirectToAction("Index", "Cart");
            }
            
            // Calculate total amount
            decimal totalAmount = cartItems.Sum(c => c.Plant.Price * c.Quantity);
            
            // Create order
            var order = new Order
            {
                UserId = userId.Value,
                OrderDate = DateTime.Now,
                TotalAmount = totalAmount,
                Status = "Pending",
                PaymentStatus = "Pending",
                ShippingAddress = shippingAddress,
                PhoneNumber = phoneNumber
            };
            
            _context.Orders.Add(order);
            await _context.SaveChangesAsync();
            
            // Create order items and update plant quantities
            foreach (var cartItem in cartItems)
            {
                var orderItem = new OrderItem
                {
                    OrderId = order.OrderId,
                    PlantId = cartItem.PlantId,
                    Quantity = cartItem.Quantity,
                    UnitPrice = cartItem.Plant.Price,
                    TotalPrice = cartItem.Plant.Price * cartItem.Quantity
                };
                
                _context.OrderItems.Add(orderItem);
                
                // Update plant quantity
                cartItem.Plant.QuantityAvailable -= cartItem.Quantity;
            }
            
            // Clear cart
            _context.CartItems.RemoveRange(cartItems);
            await _context.SaveChangesAsync();
            
            // Simulate payment processing
            if (paymentMethod == "cod")
            {
                order.PaymentStatus = "Pending";
                order.Status = "Processing";
            }
            else
            {
                order.PaymentStatus = "Paid";
                order.Status = "Processing";
            }
            
            await _context.SaveChangesAsync();
            
            TempData["Success"] = $"Order #{order.OrderId} placed successfully!";
            return RedirectToAction("OrderConfirmation", new { orderId = order.OrderId });
        }

        // GET: Order/OrderConfirmation
        public async Task<IActionResult> OrderConfirmation(int orderId)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
                
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Plant)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
                
            if (order == null) return NotFound();
            
            return View(order);
        }

        // GET: Order/MyOrders
        public async Task<IActionResult> MyOrders()
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
                
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var orders = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Plant)
                .Where(o => o.UserId == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
                
            return View(orders);
        }

        // GET: Order/OrderDetails
        public async Task<IActionResult> OrderDetails(int orderId)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
                
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            
            var order = await _context.Orders
                .Include(o => o.OrderItems)
                .ThenInclude(oi => oi.Plant)
                .FirstOrDefaultAsync(o => o.OrderId == orderId && o.UserId == userId);
                
            if (order == null) return NotFound();
            
            return View(order);
        }
    }
} 