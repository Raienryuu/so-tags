using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SO_tags.DTOs;
using SO_tags.Models;
using SO_tags.Providers;

namespace SO_tags.Controllers;

[Route("[controller]")]
public class TagsController(
  IRemoteTagsProvider remoteTagsProvider,
  LocalTagsContext db,
  ILogger<TagsController> logger) : Controller
{
  /// <summary>
  /// Removes all tags from local storage
  /// </summary>
  /// <returns></returns>
  [HttpGet]
  [Route("reloadAllTags")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> ReloadAllTags()
  {
    await db.ClearDatabase();
    return Ok();
  }

  /// <summary>
  /// Get single page of tags
  /// </summary>
  /// <param name="args.Order"
  /// Description="Sort order:\n * `asc` - Ascending\n * `desc` - Descending\n"></param>
  /// <returns></returns>
  [HttpGet]
  [Route("getPage")]
  [ProducesResponseType(typeof(IEnumerable<Tag>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> GetPage([FromQuery] QueryFilter args)
  {
    var cache = new PageRequester(db, GetSortType(args), remoteTagsProvider,
      logger, args.PageNumber, args.PageSize);
    try
    {
      var tags = await cache.GetPage();

      if (tags.Count == 0) return BadRequest("No tags found on given page.");
      return Ok(tags);
    }
    catch (ArgumentException ex)
    {
      logger.LogError(ex, "");
      return BadRequest(ex.Message);
    }
  }

  private TagsSort GetSortType(QueryFilter args)
  {
    switch (args.Sort)
    {
      case "name":
        if (args.Order?.ToLowerInvariant() == "desc")
          return TagsSort.NameDesc;
        return TagsSort.NameAsc;
      case "share":
        if (args.Order?.ToLowerInvariant() == "desc")
          return TagsSort.ShareDesc;
        return TagsSort.ShareAsc;
    }

    throw new ArgumentException("Invalid filter values provided.");
  }
}