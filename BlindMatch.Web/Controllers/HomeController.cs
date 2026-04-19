using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using BlindMatch.Web.Models;
using BlindMatch.Web.Data;

namespace BlindMatch.Web.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;
        private readonly ApplicationDbContext _context;

        // We inject the Database Context here so the Home Page can count the stats!
        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            // 1. Calculate the live statistics from the SQL Database
            var totalProjects = await _context.ProjectProposals.CountAsync();
            var pendingProjects = await _context.ProjectProposals.CountAsync(p => p.Status == "Pending");
            var matchedProjects = await _context.ProjectProposals.CountAsync(p => p.Status == "Matched");

            // 2. Package the numbers into the ViewBag to send to the HTML page
            ViewBag.Total = totalProjects;
            ViewBag.Pending = pendingProjects;
            ViewBag.Matched = matchedProjects;

            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}