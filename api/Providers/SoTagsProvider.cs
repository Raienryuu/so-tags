using System.Net;
using System.Text.Json;
using SO_tags.DTOs;
using SO_tags.Models;
using SO_tags.TagsSorting;

namespace SO_tags.Providers;

public class SoTagsProvider : IRemoteTagsProvider
{
  private const string ApiUrl = "https://api.stackexchange.com/2.3/tags";
  private const int PageSize = 100;

  private readonly HttpClient _httpClient;
  private readonly ILogger<SoTagsProvider> _logger;
  private readonly IConfiguration _configuration;

  public SoTagsProvider(
    ILogger<SoTagsProvider> logger,
    IConfiguration configuration,
    HttpClient? httpClient = null)
  {
    _configuration = configuration;
    _logger = logger;
    var decompressionHandle = new HttpClientHandler
    {
      AutomaticDecompression =
        DecompressionMethods.GZip | DecompressionMethods.Deflate
    };
    _httpClient = httpClient ?? new HttpClient(decompressionHandle);
  }

  public async Task<IEnumerable<Tag>> GetSinglePage(int pageNumber,
    int pageSize,
    TagsSort sort)
  {
    _logger.LogInformation("Getting single page of tags from remote API.");

    var result = await TryGetSinglePage(
      pageNumber, pageSize, sort);

    return result.Items;
  }

  private static void HandleBackoff(int timer)
  {
    Thread.Sleep(timer * 1100);
  }

  private async Task<StackExchangeResponseWrapper<Tag>> TryGetSinglePage(
    int pageNumber, int pageSize = PageSize, TagsSort sort = TagsSort.NameAsc)
  {
    try
    {
      var request = GetRequestMessage(pageNumber, pageSize, sort);
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
      bool isBackedOff;
      do
      {
        var response = await _httpClient.GetAsync(request.RequestUri);

        var jsonResponse = await response.Content.ReadAsStringAsync();
        var deserializedResponse =
          JsonSerializer.Deserialize<StackExchangeResponseWrapper<Tag>>(
            jsonResponse) ??
          throw new JsonException();

        ValidateResponse(deserializedResponse);
        if (deserializedResponse.Backoff is not null)
        {
          HandleBackoff((int)deserializedResponse.Backoff);
          isBackedOff = true;
          _logger.LogError(
            "Receiver backoff for {backoff} seconds while requesting:" +
            "{uri}", deserializedResponse.Backoff, request.RequestUri);
        }
        else
        {
          return deserializedResponse;
        }
      } while (isBackedOff);
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

    return null;
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


  private HttpRequestMessage GetRequestMessage(int pageNumber,
    int pageSize = PageSize, TagsSort sort = TagsSort.NameAsc)
  {
    var key = _configuration["Api:StackExchange:key"]!;
    var filter = _configuration["Api:StackExchange:filter"]!;
    var sortString = sort.ToSortString();
    var orderString = sort.ToOrderString();
    return
      new HttpRequestMessage(HttpMethod.Get,
        ApiUrl +
        $"?key={key}&site=stackoverflow&page={pageNumber}&pagesize={pageSize}" +
        $"&&filter={filter}&sort={sortString}&order={orderString}");
  }
}