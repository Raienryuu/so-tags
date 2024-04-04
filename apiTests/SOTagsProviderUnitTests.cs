using System.Net;
using System.Text.Json;
using System.Text.Json.Nodes;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding.Binders;
using Microsoft.Extensions.Logging.Abstractions;
using Moq;
using SO_tags;
using SO_tags.Controllers;
using SO_tags.DTOs;
using SO_tags.Models;
using SO_tags.Providers;
using SO_tagsTests;
using SO_tagsTests.Fakes;

namespace apiTests;

public class SoTagsProviderUnitTests
{
  [Fact]
  public async void GetAllTags_ValidRequest_OkResponse()
  {
    // arrange
    var configuration = new Mock<Microsoft.Extensions.Configuration.IConfiguration>();
    configuration.SetupGet(x => x["key"]).Returns("blank");
    var db = await new FakeLocalTagsContextBuilder().Build();
    var httpClient = new FakeHttpClient(new FakeHttpMessageHandler());
    var cut = new SoTagsProvider(db, NullLogger<SoTagsProvider>.Instance,
      configuration.Object, httpClient);

    // act
    var response = cut.GetAllTags();
    await response;
    // assert
    Assert.True(response.IsCompletedSuccessfully);
  }

  [Fact]
  public async void GetPage_DefaultFilters_TagsPage()
  {
    var remoteTagsProvider = Mock.Of<IRemoteTagsProvider>();
    var dbContext =  await new FakeLocalTagsContextBuilder().Build();
    var cut = new TagsController(remoteTagsProvider, dbContext, NullLogger<TagsController>.Instance);
    var defaultFilters = new QueryFilter();

    var response = await cut.GetPage(defaultFilters);
    
    Assert.True(((OkObjectResult)response).StatusCode == 200);
  }
}