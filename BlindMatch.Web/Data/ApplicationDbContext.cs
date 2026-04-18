using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using BlindMatch.Web.Models;

namespace BlindMatch.Web.Data
{
    public class ApplicationDbContext : IdentityDbContext
    {
        public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
            : base(options) { }

        // Added '= default!;' to satisfy the strict nullability compiler warning
        public DbSet<ProjectProposal> ProjectProposals { get; set; } = default!;
    }
}