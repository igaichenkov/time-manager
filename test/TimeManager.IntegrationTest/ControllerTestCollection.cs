using Xunit;

namespace TimeManager.IntegrationTest
{
    [CollectionDefinition(nameof(ControllerTestCollection))]
    public class ControllerTestCollection : ICollectionFixture<TestServerFixture>
    {
    }
}
