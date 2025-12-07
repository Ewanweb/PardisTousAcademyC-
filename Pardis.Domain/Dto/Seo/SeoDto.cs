namespace Pardis.Domain.Dto
{
    public partial class Dtos
    {
        // --- ورودی‌ها (Requests) ---

        public class SeoDto
        {
            public string? MetaTitle { get; set; }
            public string? MetaDescription { get; set; }
            public string? CanonicalUrl { get; set; }
            public bool NoIndex { get; set; }
            public bool NoFollow { get; set; }
        }

        public class RecentActivityDto
        {
            public string Id { get; set; }
            public string Type { get; set; }   // "course", "user", "category"
            public string Title { get; set; }
            public string Subtitle { get; set; }
            public DateTime Time { get; set; }
        }


    }
}
