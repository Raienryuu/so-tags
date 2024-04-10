using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SO_tags.Controllers;
using SO_tags.DTOs;
using SO_tags.Providers;
using SO_tagsTests.Fakes;

namespace apiTests;

public class SoTagsProviderUnitTests
{
  [Fact]
  public async void GetPage_DefaultFilters_TagsPage()
  {
    var remoteTagsProvider = Mock.Of<IRemoteTagsProvider>();
    var dbContext = await new FakeLocalTagsContextBuilder().Build();
    var cut = new TagsController(remoteTagsProvider, dbContext, NullLogger<TagsController>.Instance);
    var defaultFilters = new QueryFilter();

    var response = await cut.GetPage(defaultFilters);

    Assert.True(((OkObjectResult)response).StatusCode == 200);
  }
  
  [Fact]
  public async void GetPage_InvalidPageNumber_BadRequestResult()
  {
    var remoteTagsProvider = Mock.Of<IRemoteTagsProvider>();
    var dbContext = await new FakeLocalTagsContextBuilder().Build();
    var cut = new TagsController(remoteTagsProvider, dbContext, NullLogger<TagsController>.Instance);
    var defaultFilters = new QueryFilter() { PageNumber = 0 };

    var response = await cut.GetPage(defaultFilters);

    Assert.True(((BadRequestObjectResult)response).StatusCode == 400);
  }
}