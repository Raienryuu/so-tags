using Microsoft.AspNetCore.Mvc.Testing;
using SO_tags;

namespace apiTests
{
    public class SOTagsIntergrationTests
    {
        readonly TagsApiFactory app = new();

        [Fact]
        public async Task RemoveTagsInLocalStorage() 
        {
            var httpClient = app.CreateClient(); 

            var response = await httpClient.GetAsync("Tags/removeAllTags"); 

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