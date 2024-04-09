using System.Text.Json.Serialization;
using SO_tags.Models;

namespace SO_tags.DTOs;

public record StackExchangeResponseWrapper<T>
{
  [JsonPropertyName("backoff")] public int? Backoff { get; set; }
  [JsonPropertyName("error_id")] public int? ErrorId { get; set; }
  [JsonPropertyName("error_message")] public string? ErrorMessage { get; set; }
  [JsonPropertyName("error_name")] public string? ErrorName { get; set; }
  [JsonPropertyName("has_more")] public bool HasMore { get; set; }
  [JsonPropertyName("items")] public List<T> Items { get; set; }
  [JsonPropertyName("page")] public int? Page { get; set; }
  [JsonPropertyName("page_size")] public int? PageSize { get; set; }
  [JsonPropertyName("quota_max")] public int QuotaMax { get; set; }
  [JsonPropertyName("quota_remaining")] public int QuotaRemaining { get; set; }
  [JsonPropertyName("total")] public int? Total { get; set; }
  [JsonPropertyName("type")] public string? Type { get; set; }
}
