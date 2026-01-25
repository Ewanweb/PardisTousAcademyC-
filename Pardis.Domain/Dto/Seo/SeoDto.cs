namespace Pardis.Domain.Dto
{
        public class SeoDto
        {
            public string? MetaTitle { get; set; }
            public string? MetaDescription { get; set; }
            public string? CanonicalUrl { get; set; }
            public bool NoIndex { get; set; }
            public bool NoFollow { get; set; }
        }
}
