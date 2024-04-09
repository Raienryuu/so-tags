using Microsoft.EntityFrameworkCore;
using SO_tags.DTOs;
using SO_tags.Models;
using SO_tags.Providers;

namespace SO_tags;

public enum TagsSort
{
  NameAsc,
  NameDesc,
  ShareAsc,
  ShareDesc
}

public class PageRequester(
  LocalTagsContext db,
  TagsSort sortOrder,
  IRemoteTagsProvider remoteTags,
  ILogger logger,
  int pageNumber = 1,
  int pageSize = 20)
{
  private readonly int _initialPosition = pageNumber * pageSize;

  public async Task<List<TagDTO>> GetPage()
  {
    var tagsFromDb = await GetTagsQuery().Take(pageSize)
      .ToListAsync();
    logger.LogInformation("Fetched {tags} from local storage.",
      tagsFromDb.Count);

    if (tagsFromDb.Count < pageSize)
    {
      var missingTags = GetMissingTagsPositions(tagsFromDb);
      var tagsFromRemote = await GetTagsFromRemote(missingTags);

      AddOrUpdateTags(tagsFromRemote);
      await db.SaveChangesAsync();

      tagsFromDb.AddRange(tagsFromRemote);
      await db.RecalculateAllTagsPercentageShare();
    }

    var requestedRange = SelectOnlyRequestedRange(tagsFromDb)
      .Select(x => new TagDTO()
      {
        Count = x.Count,
        Name = x.Name,
        LocalTagsPercentage = x.LocalTagsPercentage
      })
      .Take(pageSize)
      .ToList();
    return requestedRange;
  }

  private IOrderedEnumerable<Tag> SelectOnlyRequestedRange(
    List<Tag> combinedTags)
  {
    switch (sortOrder)
    {
      case TagsSort.NameAsc:
        return combinedTags.OrderBy(x => x.NameAscPosition);
      case TagsSort.NameDesc:
        return combinedTags.OrderBy(x => x.NameDescPosition);
      case TagsSort.ShareAsc:
        return combinedTags.OrderBy(x => x.ShareAscPosition);
      case TagsSort.ShareDesc:
        return combinedTags.OrderBy(x => x.ShareDescPosition);
      default:
        return combinedTags.OrderBy(x => x.NameAscPosition);
    }
  }


  private List<int> GetMissingTagsPositions(List<Tag> localTags)
  {
    var tagsToGetFromRemote =
      Enumerable.Range(_initialPosition, pageSize)
        .ToList();

    foreach (var tag in localTags)
      switch (sortOrder)
      {
        case TagsSort.NameAsc:
          tagsToGetFromRemote.Remove((int)tag.NameAscPosition!);
          break;
        case TagsSort.NameDesc:
          tagsToGetFromRemote.Remove((int)tag.NameDescPosition!);
          break;
        case TagsSort.ShareAsc:
          tagsToGetFromRemote.Remove((int)tag.ShareAscPosition!);
          break;
        case TagsSort.ShareDesc:
          tagsToGetFromRemote.Remove((int)tag.ShareDescPosition!);
          break;
        default:
          tagsToGetFromRemote.Remove((int)tag.NameAscPosition!);
          break;
      }

    return tagsToGetFromRemote;
  }

  private async Task<List<Tag>> GetTagsFromRemote(
    ICollection<int> missingTagsPositions)
  {
    if (missingTagsPositions.Count == 0) return [];

    List<Tag> tags = [];
    for (var i = missingTagsPositions.First();
         i <= missingTagsPositions.Last();
         i++)
    {
      var startPosition = missingTagsPositions.ElementAt(0);
      startPosition -= startPosition % 20;
      var tagsRange = await GetTagsRange(startPosition);

      if (tagsRange.Count == 0) return tags;

      AssignSortPosition(tagsRange, startPosition);

      tags.AddRange(tagsRange);

      RemoveFoundTagsFromMissing(tagsRange, missingTagsPositions);
      if (missingTagsPositions.Count == 0) break;
      i = missingTagsPositions.First();
    }

    logger.LogInformation("Fetched {tags} from remote API.", tags.Count);
    return tags;
  }

  private void RemoveFoundTagsFromMissing(IEnumerable<Tag> tagsRange,
    ICollection<int> missingTagsPositions)
  {
    foreach (var tag in tagsRange)
      switch (sortOrder)
      {
        case TagsSort.NameAsc:
          missingTagsPositions.Remove((int)tag.NameAscPosition!);
          break;
        case TagsSort.NameDesc:
          missingTagsPositions.Remove((int)tag.NameDescPosition!);
          break;
        case TagsSort.ShareAsc:
          missingTagsPositions.Remove((int)tag.ShareAscPosition!);
          break;
        case TagsSort.ShareDesc:
          missingTagsPositions.Remove((int)tag.ShareDescPosition!);
          break;
        default:
          missingTagsPositions.Remove((int)tag.NameAscPosition!);
          break;
      }
  }

  private async Task<List<Tag>> GetTagsRange(int startPosition)
  {
    const int size = 20;
    const int stackExchangePageOffset = 1;
    var page = startPosition / size + stackExchangePageOffset;
    var tagsRange = await remoteTags
      .GetSinglePage(page, size, sortOrder);
    return tagsRange.ToList();
  }

  private async void AddOrUpdateTags(IReadOnlyList<Tag> tags)
  {
    for (var i = 0; i < tags.Count; i++)
    {
      var tagInDb = await db.Tags.FindAsync(tags[i].Name);
      if (tagInDb is null)
      {
        await db.Tags.AddAsync(tags[i]);
      }
      else
      {
        var tagPosition = _initialPosition + i;
        SetPositionForTag(tagPosition, tagInDb);
      }
    }

    await db.SaveChangesAsync();
  }

  private IQueryable<Tag> GetTagsQuery()
  {
    var query = db.Tags.AsQueryable();
    switch (sortOrder)
    {
      case TagsSort.NameAsc:
        return query.Where(x =>
          x.NameAscPosition != null && x.NameAscPosition >= _initialPosition &&
          x.NameAscPosition < _initialPosition + pageSize).OrderBy(x =>
          x.NameAscPosition);
      case TagsSort.NameDesc:
        return query.Where(x =>
          x.NameDescPosition != null &&
          x.NameDescPosition >= _initialPosition &&
          x.NameDescPosition < _initialPosition + pageSize).OrderBy(x =>
          x.NameDescPosition);
      case TagsSort.ShareAsc:
        return query.Where(x =>
          x.ShareAscPosition != null &&
          x.ShareAscPosition >= _initialPosition &&
          x.ShareAscPosition < _initialPosition + pageSize).OrderBy(x =>
          x.ShareAscPosition);
      case TagsSort.ShareDesc:
        return query.Where(x =>
          x.ShareDescPosition != null &&
          x.ShareDescPosition >= _initialPosition &&
          x.ShareDescPosition < _initialPosition + pageSize).OrderBy(x =>
          x.ShareDescPosition);
      default:
        return query.Where(x =>
          x.NameAscPosition != null && x.NameAscPosition >= _initialPosition &&
          x.NameAscPosition < _initialPosition + pageSize).OrderBy(x =>
          x.NameAscPosition);
    }
  }

  private void SetPositionForTag(int tagPosition, Tag tag)
  {
    switch (sortOrder)
    {
      case TagsSort.NameAsc:
        tag.NameAscPosition = tagPosition;
        break;
      case TagsSort.NameDesc:
        tag.NameDescPosition = tagPosition;
        break;
      case TagsSort.ShareAsc:
        tag.ShareAscPosition = tagPosition;
        break;
      case TagsSort.ShareDesc:
        tag.ShareDescPosition = tagPosition;
        break;
      default:
        tag.NameAscPosition = tagPosition;
        break;
    }
  }

  private void AssignSortPosition(List<Tag> tags,
    int initialPosition)
  {
    for (var i = 0; i < tags.Count; i++)
      switch (sortOrder)
      {
        case TagsSort.NameAsc:
          tags[i].NameAscPosition = initialPosition + i;
          break;
        case TagsSort.NameDesc:
          tags[i].NameDescPosition = initialPosition + i;
          break;
        case TagsSort.ShareAsc:
          tags[i].ShareAscPosition = initialPosition + i;
          break;
        case TagsSort.ShareDesc:
          tags[i].ShareDescPosition = initialPosition + i;
          break;
        default:
          tags[i].NameAscPosition = initialPosition + i;
          break;
      }
  }
}