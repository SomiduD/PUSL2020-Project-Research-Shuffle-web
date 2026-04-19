using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace BlindMatch.Web.Controllers
{
    // Locked down to System Administrators (or your ModuleLeader role)
    [Authorize(Roles = "ModuleLeader")]
    public class SysAdminController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public SysAdminController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // 1. View all users and their roles
        public async Task<IActionResult> Index()
        {
            var users = await _userManager.Users.ToListAsync();
            var userRoles = new Dictionary<string, string>();

            foreach (var user in users)
            {
                var roles = await _userManager.GetRolesAsync(user);
                userRoles.Add(user.Id, roles.FirstOrDefault() ?? "No Role");
            }

            ViewBag.UserRoles = userRoles;
            return View(users);
        }

        // 2. GET: Show the form to create a new user
        public IActionResult CreateUser()
        {
            return View();
        }

        // 3. POST: Actually create the user and assign the role
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateUser(string email, string password, string role)
        {
            if (ModelState.IsValid)
            {
                var newUser = new IdentityUser { UserName = email, Email = email, EmailConfirmed = true };
                var result = await _userManager.CreateAsync(newUser, password);

                if (result.Succeeded)
                {
                    // Ensure the role actually exists in the database before assigning it
                    if (!await _roleManager.RoleExistsAsync(role))
                    {
                        await _roleManager.CreateAsync(new IdentityRole(role));
                    }

                    await _userManager.AddToRoleAsync(newUser, role);
                    return RedirectToAction(nameof(Index));
                }

                foreach (var error in result.Errors)
                {
                    ModelState.AddModelError("", error.Description);
                }
            }
            return View();
        }

        // --------------------------------------------------------
        // NEW MANUAL APPROVAL & UPGRADE METHODS
        // --------------------------------------------------------

        // 4. POST: Promote a Student to a Supervisor
        [HttpPost]
        public async Task<IActionResult> PromoteToSupervisor(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, "Student");
                await _userManager.AddToRoleAsync(user, "Supervisor");
            }
            return RedirectToAction(nameof(Index));
        }

        // 5. POST: Demote a Supervisor back to a Student
        [HttpPost]
        public async Task<IActionResult> DemoteToStudent(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.RemoveFromRoleAsync(user, "Supervisor");
                await _userManager.AddToRoleAsync(user, "Student");
            }
            return RedirectToAction(nameof(Index));
        }

        // 6. POST: Assign the Student role to a brand new user who has "No Role"
        [HttpPost]
        public async Task<IActionResult> ApproveAsStudent(string id)
        {
            var user = await _userManager.FindByIdAsync(id);
            if (user != null)
            {
                await _userManager.AddToRoleAsync(user, "Student");
            }
            return RedirectToAction(nameof(Index));
        }
    }
}