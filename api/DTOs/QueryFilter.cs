namespace SO_tags.DTOs;

public record QueryFilter
{
  public string? Order { get; set; } = "desc";
  public string? Sort { get; set; } = "share";
  public int PageNumber { get; set; } = 0;
  public int PageSize { get; set; } = 10;
}
