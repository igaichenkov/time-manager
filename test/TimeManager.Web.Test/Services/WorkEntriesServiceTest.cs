using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Moq;
using TimeManager.Test.Common;
using TimeManager.Web.Data;
using TimeManager.Web.Data.Identity;
using TimeManager.Web.Data.WorkLog;
using TimeManager.Web.DbErrorHandlers;
using TimeManager.Web.Models.Identity;
using TimeManager.Web.Services;
using Xunit;

namespace TimeManager.Web.Test.Services
{
    public class WorkEntriesServiceTest
    {
        private const string UserId = "123456";
        private const string AnotherUserId = "abcd1234";

        [Fact]
        public async Task CreateAsync_InitsEmptyFields()
        {
            // Arrange
            string testUserId = Guid.NewGuid().ToString();
            using var dbContext = CreateDbContext();

            var userContextAccessorMock = new Mock<IUserContextAccessor>();
            userContextAccessorMock.Setup(m => m.GetUserId()).Returns(testUserId);

            var workEntriesService = new WorkEntriesService(dbContext, Mock.Of<IDbErrorHandler>(), userContextAccessorMock.Object);

            // Act
            var entry = new WorkEntry
            {
                HoursSpent = 5,
                Date = DateTime.Now.Date,
                Notes = "Notes"
            };

            var createdEntry = await workEntriesService.CreateAsync(entry);

            // Assert
            Assert.NotEqual(Guid.Empty, createdEntry.Id);
            Assert.Equal(testUserId, createdEntry.UserId);
            Assert.Equal(entry.Date, createdEntry.Date);
            Assert.Equal(entry.Notes, createdEntry.Notes);
            Assert.Equal(entry.HoursSpent, createdEntry.HoursSpent);
        }

        [Theory]
        [InlineData(false, UserId, AnotherUserId, UserId)]
        [InlineData(true, UserId, AnotherUserId, AnotherUserId)]
        public async Task CreateAsync_ExplicitUserId(bool isAdmin, string currentUserId, string createdByUserId, string expectedEntryUserId)
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var userContextAccessorMock = new Mock<IUserContextAccessor>();
            userContextAccessorMock.Setup(m => m.GetUserId()).Returns(currentUserId);
            userContextAccessorMock.Setup(m => m.IsAdminUser()).Returns(isAdmin);

            var workEntriesService = new WorkEntriesService(dbContext, Mock.Of<IDbErrorHandler>(), userContextAccessorMock.Object);

            // Act
            var entry = new WorkEntry
            {
                HoursSpent = 5,
                Date = DateTime.Now.Date,
                Notes = "Notes",
                UserId = createdByUserId
            };

            entry = await workEntriesService.CreateAsync(entry);

            // Assert
            Assert.Equal(expectedEntryUserId, entry.UserId);
        }

        [Fact]
        public async Task GetByIdAsync_EntryExists_ReturnsEntry()
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var user = await CreateTestUserAsync(dbContext);
            var entry = await CreateWorkEntry(dbContext, user.Id);

            var userContextAccessorMock = new Mock<IUserContextAccessor>();
            userContextAccessorMock.Setup(m => m.GetUserId()).Returns(user.Id);
            userContextAccessorMock.Setup(m => m.IsAdminUser()).Returns(false);

            var workEntriesService = new WorkEntriesService(dbContext, Mock.Of<IDbErrorHandler>(), userContextAccessorMock.Object);

            // Act
            var entryResult = await workEntriesService.GetByIdAsync(entry.Id);

            // Assert
            AssertEqual(entry, entryResult);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task GetByIdAsync_FilterByUserIdTest(bool isAdmin, bool resultShouldBeNull)
        {
            // Arrange
            using var dbContext = CreateDbContext();

            var user = await CreateTestUserAsync(dbContext);
            var anotherUser = await CreateTestUserAsync(dbContext);

            var entry = await CreateWorkEntry(dbContext, anotherUser.Id);

            var userContextAccessorMock = new Mock<IUserContextAccessor>();
            userContextAccessorMock.Setup(m => m.GetUserId()).Returns(user.Id);
            userContextAccessorMock.Setup(m => m.IsAdminUser()).Returns(isAdmin);

            var workEntriesService = new WorkEntriesService(dbContext, Mock.Of<IDbErrorHandler>(), userContextAccessorMock.Object);

            // Act
            var entryResult = await workEntriesService.GetByIdAsync(entry.Id);

            // Assert
            Assert.Equal(resultShouldBeNull, entryResult == null);
        }

        [Fact]
        public async Task FindAsync_EntriesExist_ReturnsWorkEntry()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var user = await CreateTestUserAsync(dbContext);

            var entry = await CreateWorkEntry(dbContext, user.Id);
            var entry1 = await CreateWorkEntry(dbContext, user.Id, entry => entry.Date = DateTime.Now.AddDays(-1));

            var userContextAccessorMock = new Mock<IUserContextAccessor>();
            userContextAccessorMock.Setup(m => m.GetUserId()).Returns(user.Id);
            userContextAccessorMock.Setup(m => m.IsAdminUser()).Returns(false);

            var workEntriesService = new WorkEntriesService(dbContext, Mock.Of<IDbErrorHandler>(), userContextAccessorMock.Object);

            // Act
            var entries = await workEntriesService.FindAsync(user.Id);

            // Assert
            Assert.Collection(entries,
                resultEntry => AssertEqual(entry1, resultEntry),
                resultEntry => AssertEqual(entry, resultEntry)
            );
        }

        public static IEnumerable<object[]> FilterTestsData => new List<object[]>
        {
            new object[] { true, null, null, false, 1 },
            new object[] { true, null, null, true, 3 },
            new object[] { false, null, null, true, 3 },
            new object[] { false, null, null, false, 0 },
            new object[] { false, new DateTime(2020, 3, 10), null, true, 2 },
            new object[] { false, new DateTime(2020, 3, 6), new DateTime(2020, 3, 16), true, 1 },
            new object[] { false, null, new DateTime(2020, 3, 16), true, 2 },
            new object[] { false, null, new DateTime(2020, 3, 16), false, 0 },
            new object[] { true, null, new DateTime(2020, 3, 16), false, 1 }
        };

        [Theory]
        [MemberData(nameof(FilterTestsData))]
        public async Task FindAsync_FilterTests(bool isAdmin, DateTime? minDate, DateTime? maxDate, bool sameUser, int expectedResultsCount)
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var user = await CreateTestUserAsync(dbContext);
            var anotherUser = await CreateTestUserAsync(dbContext);

            await CreateWorkEntry(dbContext, user.Id, entry => entry.Date = new DateTime(2020, 3, 5));
            await CreateWorkEntry(dbContext, user.Id, entry => entry.Date = new DateTime(2020, 3, 10));
            await CreateWorkEntry(dbContext, user.Id, entry => entry.Date = new DateTime(2020, 3, 17));
            await CreateWorkEntry(dbContext, anotherUser.Id, entry => entry.Date = new DateTime(2020, 3, 15));

            var userContextAccessorMock = new Mock<IUserContextAccessor>();
            userContextAccessorMock.Setup(m => m.GetUserId()).Returns(user.Id);
            userContextAccessorMock.Setup(m => m.IsAdminUser()).Returns(isAdmin);

            var workEntriesService = new WorkEntriesService(dbContext, Mock.Of<IDbErrorHandler>(), userContextAccessorMock.Object);

            // Act
            var entries = await workEntriesService.FindAsync(sameUser ? user.Id : anotherUser.Id, minDate, maxDate);

            // Assert
            Assert.Equal(expectedResultsCount, entries.Count);
        }

        [Fact]
        public async Task UpdateAsync_EntryExists_UpdatesEntry()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var user = await CreateTestUserAsync(dbContext);

            var entry = await CreateWorkEntry(dbContext, user.Id);

            var userContextAccessorMock = new Mock<IUserContextAccessor>();
            userContextAccessorMock.Setup(m => m.GetUserId()).Returns(user.Id);
            userContextAccessorMock.Setup(m => m.IsAdminUser()).Returns(false);

            var workEntriesService = new WorkEntriesService(dbContext, Mock.Of<IDbErrorHandler>(), userContextAccessorMock.Object);

            // Act
            var entryToUpdate = new WorkEntry
            {
                Id = entry.Id,
                Date = new DateTime(2021, 1, 1),
                HoursSpent = 6,
                UserId = user.Id + "1",
                Notes = "Some notes..."
            };

            entry = await workEntriesService.UpdateAsync(entryToUpdate);
            Assert.NotEqual(entryToUpdate.UserId, entry.UserId);

            // Line above tests that a user can't update the UserId field.
            // Now assign the same user id to compare all properties of both objects,
            // otherwise assert will fail
            entryToUpdate.UserId = entry.UserId;
            AssertEqual(entryToUpdate, entry);

            // assert entry state directly in the database
            entry = await dbContext.WorkEntries.FindAsync(entry.Id);
            AssertEqual(entryToUpdate, entry);
        }

        [Fact]
        public async Task UpdateAsync_EntryDoesNotExists_ReturnsNull()
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var user = await CreateTestUserAsync(dbContext);

            var userContextAccessorMock = new Mock<IUserContextAccessor>();
            userContextAccessorMock.Setup(m => m.GetUserId()).Returns(user.Id);
            userContextAccessorMock.Setup(m => m.IsAdminUser()).Returns(false);

            var workEntriesService = new WorkEntriesService(dbContext, Mock.Of<IDbErrorHandler>(), userContextAccessorMock.Object);

            // Act
            var entryToUpdate = new WorkEntry
            {
                Id = Guid.NewGuid(),
                Date = new DateTime(2021, 1, 1),
                HoursSpent = 6,
                UserId = user.Id + "1",
                Notes = "Some notes..."
            };

            var entry = await workEntriesService.UpdateAsync(entryToUpdate);
            Assert.Null(entry);
        }

        [Theory]
        [InlineData(true, false)]
        [InlineData(false, true)]
        public async Task UpdateAsync_AnotherUser_ReturnsNull(bool isAdmin, bool isNullResult)
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var user = await CreateTestUserAsync(dbContext);
            var anotherUser = await CreateTestUserAsync(dbContext);

            var userContextAccessorMock = new Mock<IUserContextAccessor>();
            userContextAccessorMock.Setup(m => m.GetUserId()).Returns(user.Id);
            userContextAccessorMock.Setup(m => m.IsAdminUser()).Returns(isAdmin);

            var workEntriesService = new WorkEntriesService(dbContext, Mock.Of<IDbErrorHandler>(), userContextAccessorMock.Object);
            var entry = await CreateWorkEntry(dbContext, anotherUser.Id);

            // Act
            var entryToUpdate = new WorkEntry
            {
                Id = entry.Id,
                Date = new DateTime(2021, 1, 1),
                HoursSpent = 6,
                Notes = "Some notes..."
            };

            entry = await workEntriesService.UpdateAsync(entryToUpdate);
            Assert.Equal(isNullResult, entry == null);
        }

        [Theory]
        [InlineData(true, true, true)]
        [InlineData(true, false, true)]
        [InlineData(false, false, false)]
        public async Task DeleteAsync_EntryExists_RemovesEntry(bool isAdmin, bool sameUser, bool isNullResult)
        {
            // Arrange
            using var dbContext = CreateDbContext();
            var user = await CreateTestUserAsync(dbContext);
            var anotherUser = await CreateTestUserAsync(dbContext);

            var userContextAccessorMock = new Mock<IUserContextAccessor>();
            userContextAccessorMock.Setup(m => m.GetUserId()).Returns(user.Id);
            userContextAccessorMock.Setup(m => m.IsAdminUser()).Returns(isAdmin);

            var workEntriesService = new WorkEntriesService(dbContext, Mock.Of<IDbErrorHandler>(), userContextAccessorMock.Object);
            var entry = await CreateWorkEntry(dbContext, sameUser ? user.Id : anotherUser.Id);

            // Act
            await workEntriesService.DeleteAsync(entry.Id);

            // Assert
            Assert.Equal(isNullResult, await dbContext.WorkEntries.FindAsync(entry.Id) == null);
        }

        private static ApplicationDbContext CreateDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: "TimeManager")
                .Options;

            return new ApplicationDbContext(options);
        }

        private static void AssertEqual(WorkEntry expected, WorkEntry actual)
        {
            Assert.Equal(expected.Id, actual.Id);
            Assert.Equal(expected.Date, actual.Date);
            Assert.Equal(expected.HoursSpent, actual.HoursSpent);
            Assert.Equal(expected.Notes, actual.Notes);
            Assert.Equal(expected.UserId, actual.UserId);
        }

        private static async Task<ApplicationUser> CreateTestUserAsync(ApplicationDbContext dbContext, string roleName = RoleNames.User)
        {
            var user = TestUserFactory.CreateTestUser();

            dbContext.Users.Add(user);
            await dbContext.SaveChangesAsync();

            var role = await GetOrCreateRoleAsync(dbContext, roleName);
            dbContext.UserRoles.Add(new IdentityUserRole<string>() { UserId = user.Id, RoleId = role.Id });
            await dbContext.SaveChangesAsync();

            return user;
        }

        private static async Task<IdentityRole> GetOrCreateRoleAsync(ApplicationDbContext dbContext, string roleName)
        {
            var role = await dbContext.Roles.FirstOrDefaultAsync(r => r.Name == roleName);
            if (role == null)
            {
                role = new IdentityRole(roleName);
                dbContext.Add(role);

                await dbContext.SaveChangesAsync();
            }

            return role;
        }

        private static async Task<WorkEntry> CreateWorkEntry(ApplicationDbContext dbContext, string createdBy, Action<WorkEntry> modifier = null)
        {
            var entry = new WorkEntry
            {
                Id = Guid.NewGuid(),
                HoursSpent = 5,
                Date = DateTime.Now.Date,
                Notes = "Notes",
                UserId = createdBy
            };

            if (modifier != null)
            {
                modifier(entry);
            }

            dbContext.Add(entry);
            await dbContext.SaveChangesAsync();

            return entry;
        }
    }
}