using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeManager.Web.Data.WorkLog;
using TimeManager.Web.IntegrationTest.Extensions;
using TimeManager.Web.Models.WorkEntries;
using Xunit;

namespace TimeManager.Web.IntegrationTest.Controllers
{
    public class WorkEntriesControllerTest : ControllerTestBase
    {
        private const string Notes = "note1, note2";
        private const float HoursSpent = 3.5f;

        public WorkEntriesControllerTest(TestServerFixture testServerFixture) : base(testServerFixture)
        {
        }

        [Fact]
        public async Task Post_CorrectEntryData_CreatesWorkEntry()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            await HttpClient.AuthAs(user.Email, TestPassword);

            // Act
            WorkEntryDto content = new WorkEntryDto()
            {
                Date = DateTime.UtcNow.AddYears(2).Date,
                HoursSpent = HoursSpent,
                Notes = Notes
            };

            var responseMessage = await HttpClient.PostAsync("/api/WorkEntries", content);

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);
            Assert.Equal(HttpStatusCode.Created, responseMessage.StatusCode);

            var createdEntry = await TestServerFixture.DbContext.WorkEntries.FirstAsync(e => e.UserId == user.Id);
            Assert.Equal(content.Date, createdEntry.Date);
            Assert.Equal(content.HoursSpent, createdEntry.HoursSpent);
            Assert.Equal(content.Notes, createdEntry.Notes);
        }

        [Fact]
        public async Task GetList_ReturnsListOfEntries()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var user1 = await CreateTestUserAsync();

            var entry = CreateWorkEntry(DateTime.UtcNow.Date, user.Id);
            TestServerFixture.DbContext.WorkEntries.Add(entry);
            TestServerFixture.DbContext.WorkEntries.Add(CreateWorkEntry(DateTime.UtcNow.AddDays(1).Date, user.Id));
            TestServerFixture.DbContext.WorkEntries.Add(CreateWorkEntry(DateTime.UtcNow.AddDays(2).Date, user1.Id));
            await TestServerFixture.DbContext.SaveChangesAsync();

            await HttpClient.AuthAs(user.Email, TestPassword);

            // Act
            var entries = await HttpClient.GetAsync<WorkEntryDto[]>("/api/WorkEntries");

            // Assert
            Assert.Equal(2, entries.Length);
            Assert.Equal(entry.Id, entries.First().Id);
            Assert.Equal(entry.Date, entries.First().Date);
            Assert.Equal(entry.HoursSpent, entries.First().HoursSpent);
            Assert.Equal(entry.Notes, entries.First().Notes);
        }

        [Fact]
        public async Task GetById_ReturnsSingleEntry()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            var entry = CreateWorkEntry(DateTime.UtcNow.Date, user.Id);
            TestServerFixture.DbContext.WorkEntries.Add(entry);
            await TestServerFixture.DbContext.SaveChangesAsync();

            await HttpClient.AuthAs(user.Email, TestPassword);

            // Act
            var responseEntry = await HttpClient.GetAsync<WorkEntryDto>("/api/WorkEntries/" + entry.Id);

            // Assert
            Assert.Equal(entry.Id, responseEntry.Id);
            Assert.Equal(entry.Date, responseEntry.Date);
            Assert.Equal(entry.HoursSpent, responseEntry.HoursSpent);
            Assert.Equal(entry.Notes, responseEntry.Notes);
        }

        [Fact]
        public async Task GetById_WrongId_NotFound()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            var entry = CreateWorkEntry(DateTime.UtcNow.Date, user.Id);
            TestServerFixture.DbContext.WorkEntries.Add(entry);
            await TestServerFixture.DbContext.SaveChangesAsync();

            await HttpClient.AuthAs(user.Email, TestPassword);

            // Act
            var responseMessage = await HttpClient.GetAsync("/api/WorkEntries/" + Guid.NewGuid());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        private static WorkEntry CreateWorkEntry(DateTime date, string userId)
        {
            return new WorkEntry
            {
                Date = date,
                Notes = Notes,
                HoursSpent = HoursSpent,
                UserId = userId
            };
        }
    }
}