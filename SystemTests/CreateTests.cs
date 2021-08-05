using System.Threading.Tasks;
using Xunit;

namespace SystemTests
{
    public class CreateTests : IClassFixture<ApplicationFixture>
    {
        private readonly ApplicationFixture _applicationFixture;

        public CreateTests(ApplicationFixture applicationFixture)
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
