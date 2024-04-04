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
  [HttpGet, Route("reloadAllTags")]
  [ProducesResponseType(StatusCodes.Status200OK)]
  public async Task<IActionResult> ReloadAllTags()
  {
    await remoteTagsProvider.GetAllTags();
    return Ok();
  }

  [HttpGet, Route("getPage")]
  [ProducesResponseType(typeof(IEnumerable<Tag>), StatusCodes.Status200OK)]
  [ProducesResponseType(typeof(string), StatusCodes.Status400BadRequest)]
  public async Task<IActionResult> GetPage([FromQuery] QueryFilter args)
  {
    try
    {
      var metadata = await db.Metadata.FirstOrDefaultAsync();
      if (metadata is null) await ReloadAllTags();
      var query = ApplyFilters(args);

      var tags = await query
        .Skip(args.PageNumber * args.PageSize)
        .Take(args.PageSize)
        .ToListAsync();
      if (tags.Count == 0) return BadRequest("No tags found on given page.");
      return Ok(tags);
    }
    catch (ArgumentException ex)
    {
      logger.LogError(ex, "");
      return BadRequest(ex.Message);
    }
  }

  private IQueryable<Tag> ApplyFilters(QueryFilter args)
  {
    var query = db.Tags.AsQueryable();
    switch (args.Sort)
    {
      case "name":
        if (args.Order?.ToLowerInvariant() == "desc")
          return query = query.OrderByDescending(x => x.Name);
        return query = query.OrderBy(x => x.Name);
      case "share":
        if (args.Order?.ToLowerInvariant() == "desc")
          return query = query.OrderByDescending(x => x.Count);
        return query = query.OrderBy(x => x.Count);
    }

    throw new ArgumentException("Invalid filter values provided.");
  }
}