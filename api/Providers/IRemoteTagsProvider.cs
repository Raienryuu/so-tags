using SO_tags.Models;

namespace SO_tags.Providers;

public interface IRemoteTagsProvider
{
  public Task<IEnumerable<Tag>> GetSinglePage(int pageNumber, int pageSize,
    TagsSort sort);
}