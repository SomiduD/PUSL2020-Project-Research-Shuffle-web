using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlindMatch.Web.Data;
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

        // Blind Review Dashboard: Browse proposals without seeing student details
        public async Task<IActionResult> Index()
        {
            var availableProposals = await _context.ProjectProposals
                .Where(p => p.SupervisorId == null)
                .ToListAsync();

            return View(availableProposals);
        }

        // Matching Logic: Confirm match and trigger "Identity Reveal"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ConfirmMatch(int id)
        {
            var proposal = await _context.ProjectProposals.FindAsync(id);

            if (proposal == null)
            {
                return NotFound();
            }

            // placeholder for current supervisor
            proposal.SupervisorId = "Supervisor_Anton_J";
            proposal.Status = "Matched";

            _context.Update(proposal);
            await _context.SaveChangesAsync();

            return RedirectToAction(nameof(Index));
        }
    }
}