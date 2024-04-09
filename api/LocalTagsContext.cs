using Microsoft.EntityFrameworkCore;
using SO_tags.Models;

namespace SO_tags;

public class LocalTagsContext : DbContext
{
  private readonly ILogger<LocalTagsContext> _logger;

  public LocalTagsContext(ILogger<LocalTagsContext> logger)
  {
    _logger = logger;
  }

  protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
  {
    base.OnConfiguring(optionsBuilder);
    optionsBuilder.UseSqlite("Data Source=tags.dat");
  }

  protected override void OnModelCreating(ModelBuilder modelBuilder)
  {
    base.OnModelCreating(modelBuilder);
    modelBuilder.Entity<TagsMetadata>()
      .Property(m => m.TotalTags)
      .ValueGeneratedNever();
  }


  public DbSet<Tag> Tags { get; set; }
  public DbSet<TagsMetadata> Metadata { get; set; }

  public virtual async Task ClearDatabase()
  {
    await Metadata.ExecuteDeleteAsync();
    await Tags.ExecuteDeleteAsync();
    await SaveChangesAsync();
    _logger.LogInformation("All database records have been removed.");
  }

  public async Task RecalculateAllTagsPercentageShare()
  {
    const int batchSize = 500;
    var hasMore = true;
    var totalTagsInDb =
      await Metadata.Select(m => m.TotalTags).FirstOrDefaultAsync();
    if (totalTagsInDb == 0)
    {
      totalTagsInDb = await Tags.SumAsync(x => x.Count);
    }

    var rowsUpdated = 0;
    while (hasMore)
    {
      var batch = await Tags.OrderBy(t => t.Count)
        .Skip(rowsUpdated)
        .Take(batchSize)
        .ToListAsync();

      foreach (var tag in batch)
      {
        try
        {
          tag.LocalTagsPercentage = tag.Count / (double)totalTagsInDb;
          Tags.Update(tag);
        }
        catch (DivideByZeroException _)
        {
          _logger.LogInformation(
            "Attempt to recalculate tags percentage share failed. No tags in local storage.");
        }
      }

      rowsUpdated += await SaveChangesAsync();
      if (batch.Count < batchSize) hasMore = false;
    }

    _logger.LogInformation(
      "Recalculated share of all stored locally tags for {} tags.", rowsUpdated);
  }

  public async Task RecalculateTagsPercentageShareForCollection(
    IEnumerable<Tag> tags)
  {
    var totalTagsInDb =
      await Metadata.Select(m => m.TotalTags).FirstOrDefaultAsync();
    if (totalTagsInDb == 0) return;
    var rowsUpdated = 0;
    foreach (var tag in tags)
    {
      tag.LocalTagsPercentage = (double)tag.Count / totalTagsInDb;
      Tags.Update(tag);
    }

    rowsUpdated += await SaveChangesAsync();

    _logger.LogInformation(
      "Recalculated share of all tags for {} tags.", rowsUpdated);
  }
}