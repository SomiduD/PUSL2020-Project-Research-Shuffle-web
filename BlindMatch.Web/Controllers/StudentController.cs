using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using BlindMatch.Web.Data;
using BlindMatch.Web.Models;

namespace BlindMatch.Web.Controllers
{
    // Locks down the entire student section
    [Authorize(Roles = "Student")]
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly UserManager<IdentityUser> _userManager;

        public StudentController(ApplicationDbContext context, UserManager<IdentityUser> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // 1. Dashboard: Shows only the proposals submitted by this logged-in student
        public async Task<IActionResult> Index()
        {
            var currentUser = await _userManager.GetUserAsync(User);
            if (currentUser == null) return Challenge();

            var myProposals = await _context.ProjectProposals
                .Where(p => p.StudentId == currentUser.Id)
                .ToListAsync();

            return View(myProposals);
        }

        // 2. GET: Shows the blank submission form
        public IActionResult Create()
        {
            return View();
        }

        // 3. POST: Saves the proposal and attaches the Student's ID
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectProposal proposal)
        {
            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                proposal.Status = "Pending";
                proposal.SupervisorId = null;
                proposal.StudentId = currentUser?.Id; // Automatically link the student!

                _context.Add(proposal);
                await _context.SaveChangesAsync();

                // Redirect to their new dashboard after submitting
                return RedirectToAction(nameof(Index));
            }
            return View(proposal);
        }
    }
}