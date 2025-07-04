using Microsoft.AspNetCore.Mvc;
using PlantNurseryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace PlantNurseryManagement.Controllers
{
    public class BookingsController : Controller
    {
        private readonly MyNurseryDbContext _context;
        public BookingsController(MyNurseryDbContext context)
        {
            _context = context;
        }

        private int? GetCurrentUserId()
        {
            var email = HttpContext.Session.GetString("UserEmail");
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            return user?.UserId;
        }

        // GET: Bookings/Create?plantId=5
        public async Task<IActionResult> Create(int? plantId)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
            if (plantId == null) return NotFound();
            var plant = await _context.Plants.FindAsync(plantId);
            if (plant == null) return NotFound();
            return View(plant);
        }

        // POST: Bookings/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(int plantId, int quantity)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            var plant = await _context.Plants.FindAsync(plantId);
            if (plant == null) return NotFound();
            if (quantity < 1 || quantity > plant.QuantityAvailable)
            {
                ModelState.AddModelError("", $"Quantity must be between 1 and {plant.QuantityAvailable}.");
                return View(plant);
            }
            var booking = new Booking
            {
                UserId = userId.Value,
                PlantId = plantId,
                Quantity = quantity,
                BookingDate = DateTime.Now,
                Status = "Pending"
            };
            _context.Bookings.Add(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyBookings");
        }

        // GET: Bookings/MyBookings
        public async Task<IActionResult> MyBookings()
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
            var userId = GetCurrentUserId();
            if (userId == null) return RedirectToAction("Login", "Account");
            var bookings = await _context.Bookings
                .Include(b => b.Plant)
                .Where(b => b.UserId == userId)
                .ToListAsync();
            return View(bookings);
        }

        // GET: Bookings/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();
            var booking = await _context.Bookings.Include(b => b.Plant).FirstOrDefaultAsync(b => b.BookingId == id);
            var userId = GetCurrentUserId();
            if (booking == null || booking.UserId != userId || booking.Status != "Pending") return NotFound();
            return View(booking);
        }

        // POST: Bookings/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, int quantity)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
            var booking = await _context.Bookings.Include(b => b.Plant).FirstOrDefaultAsync(b => b.BookingId == id);
            var userId = GetCurrentUserId();
            if (booking == null || booking.UserId != userId || booking.Status != "Pending") return NotFound();
            if (quantity < 1 || quantity > booking.Plant.QuantityAvailable)
            {
                ModelState.AddModelError("", $"Quantity must be between 1 and {booking.Plant.QuantityAvailable}.");
                return View(booking);
            }
            booking.Quantity = quantity;
            await _context.SaveChangesAsync();
            return RedirectToAction("MyBookings");
        }

        // GET: Bookings/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();
            var booking = await _context.Bookings.Include(b => b.Plant).FirstOrDefaultAsync(b => b.BookingId == id);
            var userId = GetCurrentUserId();
            if (booking == null || booking.UserId != userId || booking.Status != "Pending") return NotFound();
            return View(booking);
        }

        // POST: Bookings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
            var booking = await _context.Bookings.FindAsync(id);
            var userId = GetCurrentUserId();
            if (booking == null || booking.UserId != userId || booking.Status != "Pending") return NotFound();
            _context.Bookings.Remove(booking);
            await _context.SaveChangesAsync();
            return RedirectToAction("MyBookings");
        }

        // GET: Bookings/AdminList
        public async Task<IActionResult> AdminList()
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");
            var bookings = await _context.Bookings
                .Include(b => b.Plant)
                .Include(b => b.User)
                .Include(b => b.Admin)
                .ToListAsync();
            return View(bookings);
        }

        // POST: Bookings/Approve/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Approve(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null || booking.Status != "Pending") return NotFound();
            var adminEmail = HttpContext.Session.GetString("UserEmail");
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == adminEmail);
            booking.Status = "Approved";
            booking.AdminId = admin?.AdminId;
            await _context.SaveChangesAsync();
            return RedirectToAction("AdminList");
        }

        // POST: Bookings/Cancel/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Cancel(int id)
        {
            if (HttpContext.Session.GetString("UserRole") != "Admin")
                return RedirectToAction("Login", "Account");
            var booking = await _context.Bookings.FindAsync(id);
            if (booking == null || booking.Status != "Pending") return NotFound();
            var adminEmail = HttpContext.Session.GetString("UserEmail");
            var admin = await _context.Admins.FirstOrDefaultAsync(a => a.Email == adminEmail);
            booking.Status = "Cancelled";
            booking.AdminId = admin?.AdminId;
            await _context.SaveChangesAsync();
            return RedirectToAction("AdminList");
        }
    }
} 