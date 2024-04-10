using SO_tags.Models;

namespace SO_tags.TagsSorting;

public class ShareDescSort : ITagSortOrder
{
  public IOrderedEnumerable<Tag> SelectOnlyRequestedRange(
    IEnumerable<Tag> combinedTags)
  {
    return combinedTags.OrderBy(x => x.ShareDescPosition);
  }

  public void RemoveFoundTagsFromMissing(IEnumerable<Tag> tagsRange,
    ICollection<int> missingTagsPositions)
  {
    foreach (var tag in tagsRange)
      missingTagsPositions.Remove((int)tag.ShareDescPosition!);
  }

  public IQueryable<Tag> GetTagsQuery(IQueryable<Tag> query,
    int initialPosition,
    int pageSize)
  {
    return query.Where(x =>
      x.ShareDescPosition != null && x.ShareDescPosition >= initialPosition &&
      x.ShareDescPosition < initialPosition + pageSize).OrderBy(x =>
      x.ShareDescPosition);
  }

  public void SetPositionForTag(in int tagPosition, Tag tag)
  {
    tag.ShareDescPosition = tagPosition;
  }

  public void AssignSortPosition(List<Tag> tags, in int initialPosition)
  {
    for (var i = 0; i < tags.Count; i++)
      tags[i].ShareDescPosition = initialPosition + i;
  }
}