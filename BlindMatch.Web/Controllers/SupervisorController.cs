using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BlindMatch.Web.Data;
using BlindMatch.Web.Models;

namespace BlindMatch.Web.Controllers
{
    [Authorize(Roles = "Supervisor")]
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public SupervisorController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Dashboard: Shows unassigned proposals AND applies the Research Area filter
        public async Task<IActionResult> Index(string searchArea)
        {
            // Start with only the projects that haven't been claimed
            var proposalsQuery = _context.ProjectProposals
                .Where(p => p.SupervisorId == null);

            // Apply the filter if the supervisor selected a specific Research Area from the dropdown
            if (!string.IsNullOrEmpty(searchArea))
            {
                proposalsQuery = proposalsQuery.Where(p => p.ResearchArea == searchArea);
            }

            // Pass the current filter back to the View so the dropdown remembers what was selected
            ViewBag.CurrentFilter = searchArea;

            return View(await proposalsQuery.ToListAsync());
        }

        // 2. Match Logic: Assigns the logged-in supervisor's ID to the proposal
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmMatch(int proposalId) // Matches the 'name="proposalId"' in the HTML form
        {
            var proposal = await _context.ProjectProposals.FindAsync(proposalId);
            if (proposal == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            // Break the anonymity and lock the database row
            proposal.SupervisorId = currentUser.Id;
            proposal.Status = "Matched";
            proposal.MatchedAt = DateTime.Now;

            _context.Update(proposal);
            await _context.SaveChangesAsync();

            // Redirect them to their personal dashboard so they can immediately see the student's identity!
            return RedirectToAction(nameof(MyMatches));
        }

        // 3. The Identity Reveal - Shows only the projects this specific supervisor claimed
        public async Task<IActionResult> MyMatches()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var myMatches = await _context.ProjectProposals
                .Where(p => p.SupervisorId == currentUser.Id)
                .ToListAsync();

            return View(myMatches);
        }
    }
}