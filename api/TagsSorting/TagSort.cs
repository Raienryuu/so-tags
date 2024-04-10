namespace SO_tags.TagsSorting
{
  public enum TagsSort
  {
    NameAsc,
    NameDesc,
    ShareAsc,
    ShareDesc
  }
  public static class TagSorting
  {
    public static TagsSort GetSortTypeFrom(string sort, string order)
    {
      switch (sort)
      {
        case "name":
          if (order.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
            return TagsSort.NameDesc;
          return TagsSort.NameAsc;
        case "share":
          if (order.Equals("desc", StringComparison.InvariantCultureIgnoreCase))
            return TagsSort.ShareDesc;
          return TagsSort.ShareAsc;
      }

      throw new ArgumentException("Invalid filter values provided.");
    }

    public static string ToSortString(this TagsSort sort)
    {
      switch (sort)
      {
        case TagsSort.NameAsc:
        case TagsSort.NameDesc:
        default:
          return "name";
        case TagsSort.ShareAsc:
        case TagsSort.ShareDesc:
          return "popular";
      }
    }

    public static string ToOrderString(this TagsSort sort)
    {
      switch (sort)
      {
        case TagsSort.NameAsc:
        case TagsSort.ShareAsc:
          return "asc";
        case TagsSort.NameDesc:
        case TagsSort.ShareDesc:
        default:
          return "desc";
      }
    }
  }
}