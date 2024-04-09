using Microsoft.EntityFrameworkCore;

namespace SO_tags.Models;

[PrimaryKey(nameof(TotalTags))]
public record TagsMetadata
{
  public int TotalTags { get; set; } = 0;
}