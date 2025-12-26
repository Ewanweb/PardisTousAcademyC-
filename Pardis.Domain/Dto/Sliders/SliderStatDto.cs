namespace Pardis.Domain.Dto.Sliders
{
    public class SlideStatDto
    {
        public string? Icon { get; set; }
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    public class StoryStatDto
    {
        public string Value { get; set; } = string.Empty;
        public string Label { get; set; } = string.Empty;
    }

    public class SlideActionDto
    {
        public string Label { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }

    public class StoryActionDto
    {
        public string Label { get; set; } = string.Empty;
        public string Link { get; set; } = string.Empty;
    }
}