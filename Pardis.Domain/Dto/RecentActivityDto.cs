namespace Pardis.Domain.Dto;

public class RecentActivityDto
{
    public string Id { get; set; }
    public string Type { get; set; }   // "course", "user", "category"
    public string Title { get; set; }
    public string Subtitle { get; set; }
    public DateTime Time { get; set; }
}