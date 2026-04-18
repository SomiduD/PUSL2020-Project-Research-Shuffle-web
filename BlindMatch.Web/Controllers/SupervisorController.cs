using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlindMatch.Web.Data; // Ensure this matches your folder name
using BlindMatch.Web.Models;

namespace BlindMatch.Web.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupervisorController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var proposals = await _context.ProjectProposals.ToListAsync();
            return View(proposals);
        }
    } // This bracket closes the class
} // This bracket closes the namespace