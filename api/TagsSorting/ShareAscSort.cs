using SO_tags.Models;

namespace SO_tags.TagsSorting;

public class ShareAscSort : ITagSortOrder
{
  public IOrderedEnumerable<Tag> SelectOnlyRequestedRange(
    IEnumerable<Tag> combinedTags)
  {
    return combinedTags.OrderBy(x => x.ShareAscPosition);
  }

  public void RemoveExistingTag(in ICollection<Tag> localTags,
    ICollection<int> tagsToGetFromRemote)
  {
    foreach (var tag in localTags)
      tagsToGetFromRemote.Remove((int)tag.ShareAscPosition!);
  }

  public void RemoveFoundTagsFromMissing(IEnumerable<Tag> tagsRange,
    ICollection<int> missingTagsPositions)
  {
    foreach (var tag in tagsRange)
      missingTagsPositions.Remove((int)tag.ShareAscPosition!);
  }

  public IQueryable<Tag> GetTagsQuery(IQueryable<Tag> query,
    int initialPosition,
    int pageSize)
  {
    return query.Where(x =>
      x.ShareAscPosition != null && x.ShareAscPosition >= initialPosition &&
      x.ShareAscPosition < initialPosition + pageSize).OrderBy(x =>
      x.ShareAscPosition);
  }

  public void SetPositionForTag(in int tagPosition, Tag tag)
  {
    tag.ShareAscPosition = tagPosition;
  }

  public void AssignSortPosition(List<Tag> tags, in int initialPosition)
  {
    for (var i = 0; i < tags.Count; i++)
      tags[i].ShareAscPosition = initialPosition + i;
  }
}