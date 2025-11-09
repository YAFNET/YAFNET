using System.Text.Json.Serialization;

namespace WebEssentials.AspNetCore.Pwa;

/// <summary>
/// A related native application.
/// </summary>
public class RelatedApplication
{
    /// <summary>The platform on which the application can be found.</summary>
    [JsonPropertyName("platform")]
    public string Platform { get; set; }

    /// <summary>The URL at which the application can be found.</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }

    /// <summary>The ID used to represent the application on the specified platform.</summary>
    [JsonPropertyName("id")]
    public string Id { get; set; }
}