using System.ComponentModel.DataAnnotations;

namespace Identity.Api;

public class JwtOptions
{
    [Required] public string Key { get; set; } = String.Empty;
    [Required] public string Issuer { get; set; } = "https://id.andhernand.com";
    [Required] public string Audience { get; set; } = "https://golfleague.andhernand.com/api";
}