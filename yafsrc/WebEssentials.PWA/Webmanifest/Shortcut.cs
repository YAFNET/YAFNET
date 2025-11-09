using System.Text.Json.Serialization;

namespace WebEssentials.AspNetCore.Pwa;

/// <summary>
/// A shortcut as defined in the web manifest
/// </summary>
public class Shortcut
{
    /// <summary>A string that represents the name of the shortcut, which is displayed to users in a context menu.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>A string that represents a short version of the shortcut's name. Browsers may use this in contexts where there isn't enough space to display the full name.</summary>
    [JsonPropertyName("short_name")]
    public string ShortName { get; set; }

    /// <summary>A string that describes the purpose of the shortcut. Browsers may expose this information to assistive technology, such as screen readers, which can help users understand the purpose of the shortcut.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>An app URL that opens when the associated shortcut is activated. The URL must be within the scope of the web app manifest. If the value is absolute, it should be same-origin with the page that links to the manifest file. If the value is relative, it is resolved against the manifest file's URL.</summary>
    [JsonPropertyName("url")]
    public string Url { get; set; }
}