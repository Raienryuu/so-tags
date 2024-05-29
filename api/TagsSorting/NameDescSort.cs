using SO_tags.Models;

namespace SO_tags.TagsSorting;

public class NameDescSort : ITagSortOrder
{
  public IOrderedEnumerable<Tag> OrderTags(
    IEnumerable<Tag> combinedTags)
  {
    return combinedTags.OrderBy(x => x.NameDescPosition);
  }

  public void RemoveExistingTag(in ICollection<Tag> localTags,
    ICollection<int> tagsToGetFromRemote)
  {
    foreach (var tag in localTags)
      tagsToGetFromRemote.Remove((int)tag.NameDescPosition!);
  }

  public void RemoveFoundTagsFromMissing(IEnumerable<Tag> tagsRange,
    ICollection<int> missingTagsPositions)
  {
    foreach (var tag in tagsRange)
      missingTagsPositions.Remove((int)tag.NameDescPosition!);
  }

  public IQueryable<Tag> GetTagsQuery(IQueryable<Tag> query,
    int initialPosition,
    int pageSize)
  {
    return query.Where(x =>
      x.NameDescPosition != null && x.NameDescPosition >= initialPosition &&
      x.NameDescPosition < initialPosition + pageSize).OrderBy(x =>
      x.NameDescPosition);
  }

  public void SetPositionForTag(in int tagPosition, Tag tag)
  {
    tag.NameDescPosition = tagPosition;
  }

  public void AssignSortPosition(List<Tag> tags, in int initialPosition)
  {
    for (var i = 0; i < tags.Count; i++)
      tags[i].NameDescPosition = initialPosition + i;
  }
}