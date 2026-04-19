using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using System.Security.Claims;
using BlindMatch.Web.Controllers;
using BlindMatch.Web.Data;
using BlindMatch.Web.Models;
using Xunit;

namespace BlindMatch.Tests
{
    public class StudentLogicTests
    {
        [Fact]
        public async Task Student_Submission_Should_Default_To_Pending_And_No_Supervisor()
        {
            // 1. ARRANGE
            // Setup the fake In-Memory database
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "StudentTestDB")
                .Options;
            var context = new ApplicationDbContext(options);

            // Setup the fake user manager (Moq magic!)
            var store = new Mock<IUserStore<IdentityUser>>();
            var mockUserManager = new Mock<UserManager<IdentityUser>>(store.Object, null, null, null, null, null, null, null, null);

            // Create our fake logged-in student
            var fakeStudent = new IdentityUser { Id = "Student_007", UserName = "student@test.com" };

            // Tell the mock manager: "If the controller asks who is logged in, return our fake student!"
            mockUserManager.Setup(x => x.GetUserAsync(It.IsAny<ClaimsPrincipal>())).ReturnsAsync(fakeStudent);

            // Boot up the controller with our fake database and fake user
            var controller = new StudentController(context, mockUserManager.Object);

            // Create a fresh proposal, exactly as if typed into the HTML form
            var newProposal = new ProjectProposal
            {
                Title = "AI Traffic System",
                Abstract = "A great automated traffic project.",
                TechnicalStack = "Python, TensorFlow"
            };

            // 2. ACT
            // Fire the method!
            await controller.Create(newProposal);

            // 3. ASSERT
            // Dig into the database to see what actually got saved
            var savedProposal = await context.ProjectProposals.FirstAsync();

            // Mathematically prove the business logic triggered successfully
            Assert.Equal("Pending", savedProposal.Status);               // Rule 1: Must be Pending
            Assert.Null(savedProposal.SupervisorId);                     // Rule 2: Supervisor must be hidden/null
            Assert.Equal("Student_007", savedProposal.StudentId);        // Rule 3: Must auto-attach the logged-in student's ID
        }
    }
}