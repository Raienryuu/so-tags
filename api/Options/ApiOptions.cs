using Microsoft.Extensions.Options;

namespace SO_tags.Options
{
    public class ApiOptions
    {
        public required string Key { get; set; }
        public required string Filter { get; set; }
        public required string Url { get; set; }

    }
}