using Microsoft.AspNetCore.Components.Server.ProtectedBrowserStorage;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using SO_tags;

namespace SO_tagsTests.Fakes;

public class FakeLocalTagsContext : LocalTagsContext, IDisposable
{
  public FakeLocalTagsContext(ILogger<LocalTagsContext> logger) : base(logger)
  {
    var optionsBuilder = new DbContextOptionsBuilder();
        optionsBuilder.UseInMemoryDatabase(databaseName: $"TagsTest-{Guid.NewGuid()}");
  }
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseInMemoryDatabase(databaseName: $"TagsTest-{Guid.NewGuid()}");
  }

    public override async Task ClearDatabase(){
    Metadata.RemoveRange(Metadata.Where(t => true));
    Tags.RemoveRange(Tags.Where(t => true));

    await SaveChangesAsync();
  }

}