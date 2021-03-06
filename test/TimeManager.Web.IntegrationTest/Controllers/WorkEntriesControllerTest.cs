using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using TimeManager.Test.Common;
using TimeManager.Web.Data;
using TimeManager.Web.Data.WorkLog;
using TimeManager.Web.IntegrationTest.Extensions;
using TimeManager.Web.Models.WorkEntries;
using Xunit;

namespace TimeManager.Web.IntegrationTest.Controllers
{
    public class WorkEntriesControllerTest : ControllerTestBase
    {
        public WorkEntriesControllerTest(TestServerFixture testServerFixture) : base(testServerFixture)
        {
        }

        [Fact]
        public async Task Post_CorrectEntryData_CreatesWorkEntry()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

            // Act
            WorkEntryDto content = new WorkEntryDto()
            {
                Date = DateTime.UtcNow.AddYears(2).Date,
                HoursSpent = TestWorkEntryFactory.HoursSpent,
                Notes = TestWorkEntryFactory.Notes
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
        public async Task Post_DuplicateDate_BadRequest()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

            // Act
            WorkEntryDto content = new WorkEntryDto()
            {
                Date = DateTime.UtcNow.AddYears(2).Date,
                HoursSpent = TestWorkEntryFactory.HoursSpent,
                Notes = TestWorkEntryFactory.Notes
            };

            var responseMessage = await HttpClient.PostAsync("/api/WorkEntries", content);
            Assert.True(responseMessage.IsSuccessStatusCode);

            responseMessage = await HttpClient.PostAsync("/api/WorkEntries", content);

            // Assert
            Assert.Equal(HttpStatusCode.BadRequest, responseMessage.StatusCode);
        }

        [Fact]
        public async Task GetList_ReturnsListOfEntries()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            var user1 = await CreateTestUserAsync();

            var entry = TestWorkEntryFactory.CreateWorkEntry(DateTime.UtcNow.Date, user.Id, TestServerFixture.DbContext);
            TestWorkEntryFactory.CreateWorkEntry(DateTime.UtcNow.AddDays(1).Date, user.Id, TestServerFixture.DbContext);
            TestWorkEntryFactory.CreateWorkEntry(DateTime.UtcNow.AddDays(2).Date, user1.Id, TestServerFixture.DbContext);
            await TestServerFixture.DbContext.SaveChangesAsync();

            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

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

            var entry = TestWorkEntryFactory.CreateWorkEntry(DateTime.UtcNow.Date, user.Id, TestServerFixture.DbContext);
            await TestServerFixture.DbContext.SaveChangesAsync();

            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

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

            var entry = TestWorkEntryFactory.CreateWorkEntry(DateTime.UtcNow.Date, user.Id, TestServerFixture.DbContext);
            await TestServerFixture.DbContext.SaveChangesAsync();

            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

            // Act
            var responseMessage = await HttpClient.GetAsync("/api/WorkEntries/" + Guid.NewGuid());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Put_CorrectId_UpdatesEntry()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            var entry = TestWorkEntryFactory.CreateWorkEntry(DateTime.UtcNow.Date, user.Id, TestServerFixture.DbContext);
            await TestServerFixture.DbContext.SaveChangesAsync();

            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

            // Act
            var request = new UpdateWorkEntryRequest
            {
                Date = DateTime.Now.AddYears(20),
                HoursSpent = TestWorkEntryFactory.HoursSpent + 5,
                Notes = "updated note"
            };

            var responseMessage = await HttpClient.PutAsync("/api/WorkEntries/" + entry.Id, request);

            // Assert
            Assert.Equal(HttpStatusCode.OK, responseMessage.StatusCode);
            var responseEntry = await responseMessage.ReadContentAsync<WorkEntryDto>();

            // validate response
            Assert.Equal(entry.Id, responseEntry.Id);
            Assert.Equal(request.Date, responseEntry.Date);
            Assert.Equal(request.Notes, responseEntry.Notes);
            Assert.Equal(request.HoursSpent, responseEntry.HoursSpent);

            // validate DB entry
            await TestServerFixture.DbContext.Entry(entry).ReloadAsync();
            Assert.Equal(request.Date, entry.Date);
            Assert.Equal(request.Notes, entry.Notes);
            Assert.Equal(request.HoursSpent, entry.HoursSpent);
        }

        [Fact]
        public async Task Put_WrongId_NotFound()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

            // Act
            var responseMessage = await HttpClient.GetAsync("/api/WorkEntries/" + Guid.NewGuid());

            // Assert
            Assert.Equal(HttpStatusCode.NotFound, responseMessage.StatusCode);
        }

        [Fact]
        public async Task Delete_ExistingEntry_RemovesEntry()
        {
            // Arrange
            var user = await CreateTestUserAsync();

            var entry = TestWorkEntryFactory.CreateWorkEntry(DateTime.UtcNow.Date, user.Id, TestServerFixture.DbContext);
            TestServerFixture.DbContext.WorkEntries.Add(entry);
            await TestServerFixture.DbContext.SaveChangesAsync();
            TestServerFixture.DbContext.Entry(entry).State = EntityState.Detached;

            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

            // Act
            var responseMessage = await HttpClient.DeleteAsync("/api/WorkEntries/" + entry.Id);

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);

            Assert.Null(await TestServerFixture.DbContext.WorkEntries.FindAsync(entry.Id));
        }

        [Fact]
        public async Task Delete_NonExistingEntry_RemovesEntry()
        {
            // Arrange
            var user = await CreateTestUserAsync();
            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

            // Act
            var responseMessage = await HttpClient.DeleteAsync("/api/WorkEntries/" + Guid.NewGuid());

            // Assert
            Assert.True(responseMessage.IsSuccessStatusCode);
        }
    }
}