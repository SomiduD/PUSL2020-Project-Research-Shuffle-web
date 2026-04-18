using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization; // Added for Security
using Microsoft.AspNetCore.Identity;      // Added to get the real User
using BlindMatch.Web.Data;
using BlindMatch.Web.Models;

namespace BlindMatch.Web.Controllers
{
    // This completely locks down the page. If you aren't logged in, it forces you to the Login screen!
    [Authorize]
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager; // The tool to read user data

        // We inject the UserManager into the constructor alongside the database
        public SupervisorController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        public async Task<IActionResult> Index()
        {
            var availableProposals = await _context.ProjectProposals
                .Where(p => p.SupervisorId == null)
                .ToListAsync();

            return View(availableProposals);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmMatch(int id)
        {
            var proposal = await _context.ProjectProposals.FindAsync(id);
            if (proposal == null) return NotFound();

            // 1. Get the actual user who is currently logged in and clicking the button
            var currentUser = await _userManager.GetUserAsync(User);

            if (currentUser == null) return Challenge(); // Failsafe if session expired

            // 2. Replace the hardcoded fake name with the real user's unique ID!
            proposal.SupervisorId = currentUser.Id;
            proposal.Status = "Matched";

            _context.Update(proposal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}