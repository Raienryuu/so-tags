using System.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SO_tags.DTOs;
using SO_tags.Models;
using SO_tags.Providers;
using SO_tags.TagsSorting;

namespace SO_tags;

public class PageRequester(
  LocalTagsContext db,
  TagsSort sortOrder,
  IRemoteTagsProvider remoteTags,
  ILogger logger,
  int pageNumber = 0,
  int pageSize = 20)
{
  private readonly int _initialPosition =
	(pageNumber - StackExchangePageOffset) * pageSize;

  private readonly ITagSortOrder _sortOrder = CreateSortOrder(sortOrder);

  private const int BatchSize = 20;
  private const int StackExchangePageOffset = 1;


  private static ITagSortOrder CreateSortOrder(TagsSort sortOrder)
  {
	return sortOrder switch
	{
	  TagsSort.NameAsc => new NameAscSort(),
	  TagsSort.NameDesc => new NameDescSort(),
	  TagsSort.ShareAsc => new ShareAscSort(),
	  TagsSort.ShareDesc => new ShareDescSort(),
	  _ => new NameAscSort()
	};
  }

  public async Task<List<TagDTO>> GetPage()
  {
	var tagsFromDb = await FetchTagsFromDb();
	var tagsFromRemote = await GetMissingTagsFromRemote(tagsFromDb);

	var combinedTags = tagsFromDb.Concat(tagsFromRemote);

	var requestedRange = _sortOrder.OrderTags(combinedTags)
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

  private async Task<List<Tag>> FetchTagsFromDb()
  {
	var query = db.Tags.AsQueryable();
	var tagsFromDb = await _sortOrder
	  .GetTagsQuery(query, _initialPosition, pageSize)
	  .Take(pageSize)
	  .ToListAsync();
	logger.LogInformation("Fetched {tagsCount} tags from local storage.",
	  tagsFromDb.Count);
	return tagsFromDb;
  }

  private async Task<List<Tag>> GetMissingTagsFromRemote(List<Tag> tagsFromDb)
  {
	if (tagsFromDb.Count < pageSize)
	{
	  var missingTags = GetMissingTagsPositions(tagsFromDb);
	  var tagsFromRemote = await GetTagsFromRemote(missingTags);

	  AddOrUpdateTags(tagsFromRemote);
	  await db.SaveChangesAsync();

	  await db.RecalculateAllTagsPercentageShare();
	  return tagsFromRemote;
	}
	return [];
  }

  private List<int> GetMissingTagsPositions(IEnumerable<Tag> localTags)
  {
	var tagsToGetFromRemote =
	  Enumerable.Range(_initialPosition, pageSize)
		.ToList();

	_sortOrder.RemoveFoundTagsFromMissing(localTags, tagsToGetFromRemote);

	return tagsToGetFromRemote;
  }

  private async Task<List<Tag>> GetTagsFromRemote(
	ICollection<int> missingTagsPositions)
  {
	if (missingTagsPositions.Count == 0) return [];

	List<Tag> tagsFromRemote = [];
	for (var i = missingTagsPositions.First();
		 i <= missingTagsPositions.Last();
		 i++)
	{
	  var startPosition = GetStartTagPosition(missingTagsPositions);
	  var tagsRange = await GetBatchOfTags(startPosition);

	  if (tagsRange.Count == 0) return tagsFromRemote;

	  _sortOrder.AssignSortPosition(tagsRange, startPosition);
	  _sortOrder.RemoveFoundTagsFromMissing(tagsRange, missingTagsPositions);
	  tagsFromRemote.AddRange(tagsRange);

	  if (missingTagsPositions.Count == 0) break;
	  i = missingTagsPositions.First();
	}

	logger.LogInformation("Fetched {tags} from remote API.",
	  tagsFromRemote.Count);
	return tagsFromRemote;
  }

  private static int GetStartTagPosition(IEnumerable<int> missingTagsPositions)
  {
	var startPosition = missingTagsPositions.ElementAt(0);
	startPosition -= startPosition % BatchSize;
	return startPosition;
  }

  private async Task<List<Tag>> GetBatchOfTags(int startPosition)
  {
	var page = startPosition / BatchSize + StackExchangePageOffset;
	var tagsRange = await remoteTags
	  .GetSinglePage(page, BatchSize, sortOrder);
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
		_sortOrder.SetPositionForTag(tagPosition, tagInDb);
	  }
	}

	await db.SaveChangesAsync();
  }
}