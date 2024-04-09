using System.Text.Json.Serialization;

namespace SO_tags.DTOs;

public class TagDTO
{
  [JsonPropertyName("count")] public int Count { get; init; }
  [JsonPropertyName("name")] public string Name { get; init; }

  [JsonPropertyName("local_tags_percentage")]
  public double? LocalTagsPercentage { get; set; }
}