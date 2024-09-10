namespace GolfLeague.Api.Options;

public class JwtOptions
{
    public const string SectionName = "JwtSettings";
    public string Key { get; set; } = String.Empty;
    public string Issuer { get; set; } = "https://id.andhernand.com";
    public string Audience { get; set; } = "https://golfleague.andhernand.com/api";
}