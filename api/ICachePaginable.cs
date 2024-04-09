using SO_tags.Models;

namespace SO_tags;

public interface ICachePaginable
{
  public Task<List<Tag>> GetPage();
}