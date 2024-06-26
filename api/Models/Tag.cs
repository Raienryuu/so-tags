﻿using System.Text.Json.Serialization;
using Microsoft.EntityFrameworkCore;

namespace SO_tags.Models;

[PrimaryKey(nameof(Name))]
[Index(nameof(Count))]
public class Tag
{
  [JsonPropertyName("count")] public int Count { get; init; }

  [JsonPropertyName("has_synonyms")] public bool HasSynonyms { get; init; }

  [JsonPropertyName("is_moderator_only")]
  public bool IsModeratorOnly { get; init; }

  [JsonPropertyName("is_required")] public bool IsRequired { get; init; }

  [JsonPropertyName("name")] public string Name { get; init; }

  [JsonPropertyName("synonyms")] public List<string>? Synonyms { get; init; }

  [JsonPropertyName("local_tags_percentage")]
  public double? LocalTagsPercentage { get; set; }

  [JsonPropertyName("name_asc_position")]
  public int? NameAscPosition { get; set; }

  [JsonPropertyName("name_desc_position")]
  public int? NameDescPosition { get; set; }

  [JsonPropertyName("share_asc_position")]
  public int? ShareAscPosition { get; set; }

  [JsonPropertyName("share_desc_position")]
  public int? ShareDescPosition { get; set; }
}