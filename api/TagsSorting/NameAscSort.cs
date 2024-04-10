using SO_tags.Models;

namespace SO_tags.TagsSorting;

public class NameAscSort : ITagSortOrder
{
  public IOrderedEnumerable<Tag> SelectOnlyRequestedRange(
    IEnumerable<Tag> combinedTags)
  {
    return combinedTags.OrderBy(x => x.NameAscPosition);
  }

  public void RemoveExistingTag(in ICollection<Tag> localTags,
    ICollection<int> tagsToGetFromRemote)
  {
    foreach (var tag in localTags)
      tagsToGetFromRemote.Remove((int)tag.NameAscPosition!);
  }

  public void RemoveFoundTagsFromMissing(IEnumerable<Tag> tagsRange,
    ICollection<int> missingTagsPositions)
  {
    foreach (var tag in tagsRange)
      missingTagsPositions.Remove((int)tag.NameAscPosition!);
  }

  public IQueryable<Tag> GetTagsQuery(IQueryable<Tag> query,
    int initialPosition,
    int pageSize)
  {
    return query.Where(x =>
      x.NameAscPosition != null && x.NameAscPosition >= initialPosition &&
      x.NameAscPosition < initialPosition + pageSize).OrderBy(x =>
      x.NameAscPosition);
  }

  public void SetPositionForTag(in int tagPosition, Tag tag)
  {
    tag.NameAscPosition = tagPosition;
  }

  public void AssignSortPosition(List<Tag> tags, in int initialPosition)
  {
    for (var i = 0; i < tags.Count; i++)
      tags[i].NameAscPosition = initialPosition + i;
  }
}