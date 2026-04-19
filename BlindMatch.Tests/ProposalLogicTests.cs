using Microsoft.EntityFrameworkCore;
using BlindMatch.Web.Data;
using BlindMatch.Web.Models;
using Xunit; // The testing framework

namespace BlindMatch.Tests
{
    public class ProposalLogicTests
    {
        [Fact]
        public async Task Dashboard_Should_Only_Show_Unmatched_Proposals()
        {
            // 1. ARRANGE: Set up a fake "In-Memory" database for testing
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "Test_BlindMatchDB")
                .Options;

            // Seed the fake database with 3 proposals (2 Pending, 1 Matched)
            using (var context = new ApplicationDbContext(options))
            {
                context.ProjectProposals.Add(new ProjectProposal { Id = 1, Title = "Pending 1", Abstract = "A", TechnicalStack = "C#", SupervisorId = null, Status = "Pending" });
                context.ProjectProposals.Add(new ProjectProposal { Id = 2, Title = "Pending 2", Abstract = "B", TechnicalStack = "Java", SupervisorId = null, Status = "Pending" });
                context.ProjectProposals.Add(new ProjectProposal { Id = 3, Title = "Matched 1", Abstract = "C", TechnicalStack = "Python", SupervisorId = "Supervisor123", Status = "Matched" });

                await context.SaveChangesAsync();
            }

            // 2. ACT: Run the exact LINQ logic we use in the Supervisor Dashboard
            using (var context = new ApplicationDbContext(options))
            {
                var availableProposals = await context.ProjectProposals
                    .Where(p => p.SupervisorId == null)
                    .ToListAsync();

                // 3. ASSERT: Prove the math works!

                // It should only find the 2 pending proposals
                Assert.Equal(2, availableProposals.Count);

                // Absolutely none of the returned proposals should be marked as "Matched"
                Assert.DoesNotContain(availableProposals, p => p.Status == "Matched");
            }
        }
    }
}