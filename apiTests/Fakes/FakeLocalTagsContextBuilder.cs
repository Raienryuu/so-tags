using Microsoft.Extensions.Logging.Abstractions;
using SO_tags;

namespace SO_tagsTests.Fakes;

public class FakeLocalTagsContextBuilder : IDisposable
{
  private readonly FakeLocalTagsContext _context =
    new(NullLogger<LocalTagsContext>.Instance);

  private async Task<FakeLocalTagsContextBuilder> WithTags()
  {
    await _context.Tags.AddRangeAsync(Stubs.TagsInDb);
    return this;
  }
  public async Task<LocalTagsContext> Build()
  {
    await WithTags();
    await _context.SaveChangesAsync();
    return _context;
  }

  public void Dispose()
  {
    _context.Database.EnsureDeleted();
    _context.Dispose();
  }
}