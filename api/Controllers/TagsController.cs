using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SO_tags.DTOs;
using SO_tags.Models;
using SO_tags.Providers;
using SO_tags.TagsSorting;

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
  /// <returns></returns>
  [HttpGet]
  [Route("getPage")]
  [ProducesResponseType(typeof(IEnumerable<Tag>), StatusCodes.Status200OK)]
  public async Task<IActionResult> GetPage([FromQuery] QueryFilter args)
  {
    var cache = new PageRequester(db, TagSorting.GetSortTypeFrom(args.Sort, args.Order), remoteTagsProvider,
      logger, args.PageNumber, args.PageSize);
    try
    {
      if (args.PageNumber < 1) return BadRequest("pageNumber is lower than 1");
      var tags = await cache.GetPage();

      if (tags.Count == 0) return Ok("No tags found on given page.");
      return Ok(tags);
    }
    catch (ArgumentException ex)
    {
      logger.LogError(ex, "");
      return BadRequest(ex.Message);
    }
  }

}