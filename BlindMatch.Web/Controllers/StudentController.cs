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

        // --------------------------------------------------------
        // NEW PATCH METHODS: Handling Edit and Withdraw (Delete)
        // --------------------------------------------------------

        // 4. GET: Loads the Edit Page with the existing data
        [HttpGet]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var currentUser = await _userManager.GetUserAsync(User);

            // Security check: Only load it if the student owns it AND it is still Pending
            var proposal = await _context.ProjectProposals
                .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == currentUser.Id);

            if (proposal == null || proposal.Status != "Pending") return NotFound();

            return View(proposal);
        }

        // 5. POST: Saves the changes to the database
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Abstract,TechnicalStack,ResearchArea")] ProjectProposal proposal)
        {
            if (id != proposal.Id) return NotFound();

            if (ModelState.IsValid)
            {
                var currentUser = await _userManager.GetUserAsync(User);

                // Fetch the original from the database to ensure it hasn't been tampered with
                var existingProposal = await _context.ProjectProposals
                    .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == currentUser.Id);

                if (existingProposal == null || existingProposal.Status != "Pending") return NotFound();

                // Safely update only the allowed fields
                existingProposal.Title = proposal.Title;
                existingProposal.Abstract = proposal.Abstract;
                existingProposal.TechnicalStack = proposal.TechnicalStack;
                existingProposal.ResearchArea = proposal.ResearchArea;

                _context.Update(existingProposal);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(proposal);
        }

        // 6. POST: Handles the "Withdraw" button
        [HttpPost]
        public async Task<IActionResult> Delete(int id)
        {
            var currentUser = await _userManager.GetUserAsync(User);

            var proposal = await _context.ProjectProposals
                .FirstOrDefaultAsync(p => p.Id == id && p.StudentId == currentUser.Id);

            // Security check: Only allow deletion if it belongs to them and hasn't been matched yet
            if (proposal != null && proposal.Status == "Pending")
            {
                _context.ProjectProposals.Remove(proposal);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}