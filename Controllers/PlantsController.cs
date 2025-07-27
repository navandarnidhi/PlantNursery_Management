using Microsoft.AspNetCore.Mvc;
using PlantNurseryManagement.Models;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using System.Threading.Tasks;

namespace PlantNurseryManagement.Controllers
{
    public class PlantsController : Controller
    {
        private readonly MyNurseryDbContext _context;
        public PlantsController(MyNurseryDbContext context)
        {
            _context = context;
        }

        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // GET: Plants/Shop - Public access for shopping
        public async Task<IActionResult> Shop()
        {
            var plants = await _context.Plants.Where(p => p.QuantityAvailable > 0).ToListAsync();
            return View(plants);
        }

        // GET: Plants
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View(await _context.Plants.ToListAsync());
        }

        // GET: Plants/Create
        public IActionResult Create()
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            return View();
        }

        // POST: Plants/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Name,Description,Price,ImageUrl,QuantityAvailable")] Plant plant)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (ModelState.IsValid)
            {
                _context.Add(plant);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(plant);
        }

        // GET: Plants/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();
            var plant = await _context.Plants.FindAsync(id);
            if (plant == null) return NotFound();
            return View(plant);
        }

        // POST: Plants/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PlantId,Name,Description,Price,ImageUrl,QuantityAvailable")] Plant plant)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id != plant.PlantId) return NotFound();
            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(plant);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!_context.Plants.Any(e => e.PlantId == id))
                        return NotFound();
                    else
                        throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(plant);
        }

        // GET: Plants/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            if (id == null) return NotFound();
            var plant = await _context.Plants.FirstOrDefaultAsync(m => m.PlantId == id);
            if (plant == null) return NotFound();
            return View(plant);
        }

        // POST: Plants/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin()) return RedirectToAction("Login", "Account");
            var plant = await _context.Plants.FindAsync(id);
            if (plant != null)
            {
                _context.Plants.Remove(plant);
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: Plants/UserList - For logged-in users
        public async Task<IActionResult> UserList()
        {
            if (HttpContext.Session.GetString("UserRole") != "User")
                return RedirectToAction("Login", "Account");
            var plants = await _context.Plants.Where(p => p.QuantityAvailable > 0).ToListAsync();
            return View(plants);
        }
    }
} 