namespace Pardis.Domain.Seo
{
    public interface ISeoEntity
    {
        string Slug { get; }
        string Title { get; }
        string? Description { get; }
        SeoMetadata? Seo { get; }
        SeoEntityType GetSeoEntityType();
        Dictionary<string, object> GetSeoContext();
        DateTimeOffset CreatedAt { get; }
        DateTimeOffset UpdatedAt { get; }
    }

    public enum SeoEntityType
    {
        Category = 1,
        Course = 2,
        Page = 3,
        Home = 4
    }
}