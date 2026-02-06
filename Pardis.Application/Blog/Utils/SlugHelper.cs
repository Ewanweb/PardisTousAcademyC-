using System.Text.RegularExpressions;

namespace Pardis.Application.Blog.Utils;

public static class SlugHelper
{
    public static string Normalize(string input)
    {
        var value = input?.Trim().ToLowerInvariant() ?? string.Empty;
        value = Regex.Replace(value, @"\s+", "-");
        value = Regex.Replace(value, @"[^a-z0-9\u0600-\u06FF\-]+", "");
        value = Regex.Replace(value, @"-+", "-");
        return value.Trim('-');
    }
}
