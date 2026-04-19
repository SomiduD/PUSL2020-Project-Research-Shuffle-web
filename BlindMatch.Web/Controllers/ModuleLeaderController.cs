using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlindMatch.Web.Data;

namespace BlindMatch.Web.Controllers
{
    // Strictly locked to the Module Leader / Admin role
    [Authorize(Roles = "ModuleLeader")]
    public class ModuleLeaderController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ModuleLeaderController(ApplicationDbContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // The admin sees ALL projects, regardless of status
            var allProjects = await _context.ProjectProposals.ToListAsync();
            return View(allProjects);
        }

        [HttpPost]
        public async Task<IActionResult> Unmatch(int id)
        {
            // Admin override: Forcefully break a match if there is a dispute
            var project = await _context.ProjectProposals.FindAsync(id);
            if (project != null)
            {
                project.SupervisorId = null;
                project.Status = "Pending";
                project.MatchedAt = null;
                await _context.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}