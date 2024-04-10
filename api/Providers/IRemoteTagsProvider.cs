using SO_tags.Models;
using SO_tags.TagsSorting;

namespace SO_tags.Providers;

public interface IRemoteTagsProvider
{
  public Task<IEnumerable<Tag>> GetSinglePage(int pageNumber, int pageSize,
    TagsSort sort);
}