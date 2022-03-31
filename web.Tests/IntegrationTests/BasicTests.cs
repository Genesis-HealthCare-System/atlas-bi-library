using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc.Testing;
using Xunit;

namespace web.Tests.IntegrationTests
{
    public class BasicTests : IClassFixture<CustomWebApplicationFactory<Atlas_Web.Startup>>
    {
        private readonly CustomWebApplicationFactory<Atlas_Web.Startup> _factory;

        public BasicTests(CustomWebApplicationFactory<Atlas_Web.Startup> factory)
        {
            _factory = factory;
        }

        [Theory]
        [InlineData("/")]
        [InlineData("/Collections")]
        [InlineData("/Collections/New")]
        [InlineData("/Initiatives")]
        [InlineData("/Initiatives/New")]
        [InlineData("/Terms")]
        [InlineData("/Terms/New")]
        [InlineData("/Search")]
        [InlineData("/Search?Query=test")] // > need to have a solr install for this to
        [InlineData("/about_analytics")]
        [InlineData("/Settings")]
        [InlineData("/Analytics")]
        [InlineData("/Users")] // me
        [InlineData("/Users?id=2")] // you
        [InlineData("/tasks")]
        //[InlineData("/Groups")] > requires an id
        public async Task Get_EndpointsReturnSuccessAndCorrectContentType(string url)
        {
            // Arrange
            var client = _factory.CreateClient();

            // Act
            var response = await client.GetAsync(url);

            // Assert
            response.EnsureSuccessStatusCode(); // Status Code 200-299
            // Assert.Equal(
            //     "text/html; charset=utf-8",
            //     response.Content.Headers.ContentType.ToString()
            // );
        }


    }
}
