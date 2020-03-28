using System;
using System.Threading.Tasks;
using TimeManager.Test.Common;
using TimeManager.Web.IntegrationTest.Extensions;
using Xunit;

namespace TimeManager.Web.IntegrationTest.Controllers
{
    public class ExportControllerTest : ControllerTestBase
    {
        public ExportControllerTest(TestServerFixture testServerFixture) : base(testServerFixture)
        {

        }

        [Theory]
        [InlineData("2020-03-20", "", "since 2020-03-20", 2)]
        [InlineData("2020-03-20", "2020-03-28", "2020-03-20 - 2020-03-28", 2)]
        [InlineData("", "2020-03-24", "until 2020-03-24", 2)]
        [InlineData("", "", "", 3)]
        public async Task Export_TestCases(string dateFrom, string dateTo, string expectedContent, int expectedRowsCount)
        {
            // Arrange
            var user = await CreateTestUserAsync();

            TestWorkEntryFactory.CreateWorkEntry(new DateTime(2020, 3, 21), user.Id, TestServerFixture.DbContext);
            TestWorkEntryFactory.CreateWorkEntry(new DateTime(2020, 3, 19), user.Id, TestServerFixture.DbContext);
            TestWorkEntryFactory.CreateWorkEntry(new DateTime(2020, 3, 25), user.Id, TestServerFixture.DbContext);

            await TestServerFixture.DbContext.SaveChangesAsync();

            await HttpClient.AuthAs(user.Email, TestUserFactory.TestPassword);

            // Act
            var responseMessage = await HttpClient.GetAsync($"api/Export/users/{user.Id}/work-entries?minDate={dateFrom}&maxDate={dateTo}");
            Assert.True(responseMessage.IsSuccessStatusCode);
            Assert.Equal("text/html", responseMessage.Content.Headers.ContentType.MediaType);

            string response = await responseMessage.Content.ReadAsStringAsync();
            Assert.Contains(expectedContent, response);
            Assert.Equal(expectedRowsCount + 1, CountOccurences(response, "<tr>")); // +1 because of the header row
        }

        private static int CountOccurences(string content, string subString)
        {
            int result = 0;
            int index = 0;
            while (true)
            {
                index = content.IndexOf(subString, index);
                if (index == -1)
                {
                    break;
                }

                index += subString.Length;
                result++;
            }

            return result;
        }
    }
}
