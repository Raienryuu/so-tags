﻿using Microsoft.EntityFrameworkCore;
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
    var hasMore = true;
    var totalTagsInDb =
      await Metadata.Select(m => m.TotalTags).FirstOrDefaultAsync();
    if (totalTagsInDb == 0) return;
    var rowsUpdated = 0;
    while (hasMore)
    {
      var batch = await Tags.OrderBy(t => t.Name).Skip(rowsUpdated).Take(100).ToListAsync();

      foreach (var tag in batch)
      {
        tag.AllTagsPercentage = tag.Count / totalTagsInDb;
        Tags.Update(tag);
      }
      rowsUpdated += await SaveChangesAsync();
      if (batch.Count < 100) hasMore = false;
    }

    _logger.LogInformation(
      "Recalculated share of all tags for {} tags.", rowsUpdated);
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
      tag.AllTagsPercentage = (double)tag.Count / totalTagsInDb;
      Tags.Update(tag);
    }

    rowsUpdated += await SaveChangesAsync();

    _logger.LogInformation(
      "Recalculated share of all tags for {} tags.", rowsUpdated);
  }
}