namespace Pardis.Application.Seo
{
    public class SeoContext
    {
        public string BaseUrl { get; set; } = string.Empty;
        public string Language { get; set; } = "fa";
        public string CurrentPath { get; set; } = string.Empty;
        public Dictionary<string, string> QueryParams { get; set; } = new();
        public int? Page { get; set; }
        public bool IsPreview { get; set; }
        public string? UserAgent { get; set; }
        public bool IsMobile { get; set; }
        public string? CurrentUrl { get; set; }
    }
}
