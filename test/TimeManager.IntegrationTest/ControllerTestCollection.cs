using Xunit;

namespace TimeManager.Web.IntegrationTest
{
    [CollectionDefinition(nameof(ControllerTestCollection))]
    public class ControllerTestCollection : ICollectionFixture<TestServerFixture>
    {
    }
}
