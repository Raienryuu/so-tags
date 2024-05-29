using SO_tags.Models;

namespace SO_tags.TagsSorting;

public interface ITagSortOrder
{
  public IOrderedEnumerable<Tag> OrderTags(
    IEnumerable<Tag> combinedTags);

  public void RemoveFoundTagsFromMissing(IEnumerable<Tag> tagsRange,
    ICollection<int> missingTagsPositions);

  public IQueryable<Tag> GetTagsQuery(IQueryable<Tag> query, int initialPosition,
    int pageSize);

  public void SetPositionForTag(in int tagPosition, Tag tag);

  public void AssignSortPosition(List<Tag> tags,
    in int initialPosition);
}