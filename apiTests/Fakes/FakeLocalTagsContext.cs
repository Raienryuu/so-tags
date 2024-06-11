using Microsoft.EntityFrameworkCore;
using SO_tags;


namespace SO_tagsTests.Fakes;

public class FakeLocalTagsContext : LocalTagsContext
{
  public FakeLocalTagsContext(DbContextOptions<LocalTagsContext> options,ILogger<LocalTagsContext> logger) : base(options, logger)
  {
    var optionsBuilder = new DbContextOptionsBuilder();
    optionsBuilder.UseInMemoryDatabase(databaseName: $"TagsTest-{Guid.NewGuid()}");
  }
  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    optionsBuilder.UseInMemoryDatabase(databaseName: $"TagsTest-{Guid.NewGuid()}");
  }

  public override async Task ClearDatabase()
  {
    Metadata.RemoveRange(Metadata.Where(t => true));
    Tags.RemoveRange(Tags.Where(t => true));

    await SaveChangesAsync();
  }

}