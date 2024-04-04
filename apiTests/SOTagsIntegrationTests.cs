using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using SO_tags;

namespace apiTests
{
    public class SOTagsIntergrationTests : WebApplicationFactory<Program>
    {
        readonly TagsApiFactory app = new();

        [Fact]
        public async Task ReloadAllTags()
        {
            var httpClient = app.CreateClient();

            var response = await httpClient.GetAsync("Tags/reloadAllTags");

            Assert.True(response.IsSuccessStatusCode);
        }

        [Fact]
        public async Task GetPage()
        {
            var httpClient = app.CreateClient();

            var response = await httpClient.GetAsync("Tags/getPage");

            Assert.True(response.IsSuccessStatusCode);

        }
    }
}