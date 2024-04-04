using System.Net;
using System.Text.Json;
using Microsoft.EntityFrameworkCore;
using SO_tags.DTOs;
using SO_tags.Models;

namespace SO_tags.Providers;

public class SoTagsProvider : IRemoteTagsProvider
{
  private const string ApiUrl = "https://api.stackexchange.com/2.3/tags";
  private const int PageSize = 100;

  private readonly HttpClient _httpClient;
  private readonly LocalTagsContext _tagsDb;
  private readonly ILogger<SoTagsProvider> _logger;
  private readonly IConfiguration _configuration;

  public SoTagsProvider(LocalTagsContext dbContext,
    ILogger<SoTagsProvider> logger,
    IConfiguration configuration,
    HttpClient? httpClient = null)
  {
    _configuration = configuration;
    _logger = logger;
    _tagsDb = dbContext;
    var decompressionHandle = new HttpClientHandler
    {
      AutomaticDecompression =
        DecompressionMethods.GZip | DecompressionMethods.Deflate
    };
    _httpClient = httpClient ?? new HttpClient(decompressionHandle);
  }

  public async Task GetAllTags()
  {
    await _tagsDb.ClearDatabase();
    // add back-off handling
    _logger.LogInformation("Getting all tags from remote API.");
    var currentPage = 1;
    var hasMore = true;

    while (hasMore)
    {
      var result = await TryGetSinglePage(currentPage);
      if (result.Backoff is not null) HandleBackoff((int)result.Backoff);
      hasMore = result.HasMore;
      if (result.HasMore)
      { // soft lock until dynamic loading is present
        hasMore = currentPage < 20;
      }
      currentPage += 1;
      await TryAddTagsToDatabase(result.Items);
    }
    await _tagsDb.RecalculateAllTagsPercentageShare();
  }

  private static void HandleBackoff(int timer)
  {
    Thread.Sleep(timer * 1100);
  }

  private async Task<StackExchangeResponseWrapper<Tag>> TryGetSinglePage(
    int pageNumber)
  {
    try
    {
      var request = GetRequestMessage(pageNumber);
      return await TryGetJsonResponseFrom(request);
    }
    catch (Exception e)
    {
      _logger.LogError(e, "Unable to get a single page from remote API.");
      throw;
    }
  }

  private async Task<StackExchangeResponseWrapper<Tag>> TryGetJsonResponseFrom(
    HttpRequestMessage request)
  {
    try
    {
      var response = await _httpClient.GetAsync(request.RequestUri);

      var jsonResponse = await response.Content.ReadAsStringAsync();
      var deserializedResponse =
        JsonSerializer.Deserialize<StackExchangeResponseWrapper<Tag>>(
          jsonResponse) ??
        throw new JsonException();

      ValidateResponse(deserializedResponse);

      return deserializedResponse;
    }
    catch (HttpRequestException ex)
    {
      _logger.LogCritical(ex, "Remote API response got error code.");
      throw;
    }
    catch (Exception ex)
    {
      _logger.LogCritical(ex, "Calling remote API was unsuccessful.");
      throw;
    }
  }

  private static void ValidateResponse(
    StackExchangeResponseWrapper<Tag> deserializedResponse)
  {
    if (deserializedResponse.ErrorId is null) return;
    var errorMsg =
      "ErrorID: " + deserializedResponse.ErrorId
                  + " ErrorName: " +
                  deserializedResponse.ErrorName +
                  " ErrorMessage: " +
                  deserializedResponse.ErrorMessage;
    throw new HttpRequestException(errorMsg);
  }

  private async Task TryAddTagsToDatabase(IEnumerable<Tag> tags)
  {
    tags = tags.ToArray();
    try
    {
      await _tagsDb.AddRangeAsync(tags);
      await UpdateTotalTagsCount(tags);
      await _tagsDb.SaveChangesAsync();
    }
    catch (Exception e)
    {
      _logger.LogCritical(e, "Unable to save tags in database.");
      throw;
    }
  }

  private async Task UpdateTotalTagsCount(IEnumerable<Tag> tags)
  {
    var newTagsNumber = tags.Sum(tag => tag.Count);
    var metadata = await _tagsDb.Metadata.FirstOrDefaultAsync();
    var tagsNumber = 0;
    if (metadata != null)
    {
      tagsNumber = metadata.TotalTags;
      _tagsDb.Metadata.Remove(metadata);
    }

    metadata = new TagsMetadata { TotalTags = tagsNumber + newTagsNumber };
    await _tagsDb.Metadata.AddAsync(metadata);
  }


  private HttpRequestMessage GetRequestMessage(int pageNumber)
  {
    var key = _configuration["Api:StackExchange:key"]!;
    var filter = _configuration["Api:StackExchange:filter"]!;
    return
      new HttpRequestMessage(HttpMethod.Get,
        ApiUrl +
        $"?key={key}&site=stackoverflow&page={pageNumber}&pagesize={PageSize}&&filter={filter}");
  }
}