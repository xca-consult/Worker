using System.Threading.Tasks;
using Xunit;

namespace SystemTests
{
    public class HealthTests : IClassFixture<ApplicationFixture>
    {
        private readonly ApplicationFixture _applicationFixture;

        public HealthTests(ApplicationFixture applicationFixture)
        {
            _applicationFixture = applicationFixture;
        }

        [Fact]
        public async Task TestHealth()
        {
            // Arrange
            var client = _applicationFixture.HttpClient;

            var response = await client.GetAsync("/health");
            
            Assert.True(response.IsSuccessStatusCode);
        }
    }
}
