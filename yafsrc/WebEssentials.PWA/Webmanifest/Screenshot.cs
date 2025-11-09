using System.Text.Json.Serialization;

namespace WebEssentials.AspNetCore.Pwa;

/// <summary>
/// A Screenshot as defined in the web manifest
/// </summary>
public class Screenshot
{
    /// <summary>The path to the image file. If src is a relative URL, the base URL will be the URL of the manifest. Example: "/img/icon-192x192.png"</summary>
    [JsonPropertyName("src")]
    public string Src { get; set; }

    /// <summary>A hint as to the media type of the image.The purpose of this member is to allow a user agent to quickly ignore images of media types it does not support. Example: "image/png"</summary>
    [JsonPropertyName("type")]
    public string Type { get; set; }

    /// <summary>A string that represents the screen shape of a broad class of devices to which the screenshot applies. Specify this property only when the screenshot applies to a specific screen layout. If form_factor is not specified, the screenshot is considered suitable for all screen types.</summary>
    [JsonPropertyName("form_factor")]
    public string FormFactor { get; set; }

    /// <summary>A string that represents the accessible name of the screenshot object. Keep it descriptive because it can serve as alternative text for the rendered screenshot. For accessibility, it is recommended to specify this property for every screenshot.</summary>
    [JsonPropertyName("label")]
    public string Label { get; set; }

    /// <summary>A string containing space-separated image dimensions. Example: "192x192"</summary>
    [JsonPropertyName("sizes")]
    public string Sizes { get; set; }
}