using Microsoft.AspNetCore.Mvc;
using BlindMatch.Web.Data;
using BlindMatch.Web.Models;

namespace BlindMatch.Web.Controllers
{
    // [Authorize(Roles = "Student")] // Keep this commented out until we seed the user roles!
    public class StudentController : Controller
    {
        private readonly ApplicationDbContext _context;

        public StudentController(ApplicationDbContext context)
        {
            _context = context;
        }

        // 1. GET: Shows the blank HTML form to the user
        public IActionResult Create()
        {
            return View();
        }

        // 2. POST: Catches the form data when the user clicks "Submit"
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProjectProposal proposal)
        {
            if (ModelState.IsValid)
            {
                // Force default values to ensure the Anonymity Constraint
                proposal.Status = "Pending";
                proposal.SupervisorId = null;

                // Save to the database
                _context.Add(proposal);
                await _context.SaveChangesAsync();

                // Redirect back to the home page (or a success page)
                return RedirectToAction("Index", "Home");
            }

            // If the form has errors, return the form so they can fix it
            return View(proposal);
        }
    }
}