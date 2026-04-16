using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlindMatch.Web.Data;

namespace BlindMatch.Web.Controllers
{
    public class SupervisorController : Controller
    {
        private readonly ApplicationDbContext _context;

        public SupervisorController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. Blind Review Dashboard: Browse proposals without student details
        public async Task<IActionResult> Index()
        {
            // Only show proposals that are NOT yet matched
            var availableProposals = await _context.ProjectProposals
                .Where(p => p.SupervisorId == null)
                .ToListAsync();
            return View(availableProposals);
        }

        // 2. Matching Logic: Confirm match and trigger "Identity Reveal"
        [HttpPost]
        public async Task<IActionResult> ConfirmMatch(int id)
        {
            var proposal = await _context.ProjectProposals.FindAsync(id);
            if (proposal != null)
            {
                // In a real system, use the logged-in supervisor's ID
                proposal.SupervisorId = "Supervisor_Anton_J";
                proposal.Status = "Matched";

                await _context.SaveChangesAsync();
                [cite_start]// The identity reveal happens now because SupervisorId is no longer null 
            }
            return RedirectToAction(nameof(Index));
        }
    }
}