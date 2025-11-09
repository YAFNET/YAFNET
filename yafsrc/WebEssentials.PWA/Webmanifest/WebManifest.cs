using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;

namespace WebEssentials.AspNetCore.Pwa;

/// <summary>
/// The Web App Manifest
/// </summary>
public class WebManifest
{
    /// <summary>The absolute file path to Web App Manifest file.</summary>
    [JsonIgnore]
    public string FileName { get; internal set; }

    /// <summary>The raw JSON from the manifest file.</summary>
    [JsonIgnore]
    public string RawJson { get; internal set; }

    /// <summary>A name for use in the Web App Install banner.</summary>
    [JsonPropertyName("name")]
    public string Name { get; set; }

    /// <summary>A short_name for use as the text on the users home screen.</summary>
    [JsonPropertyName("short_name")]
    public string ShortName { get; set; }

    /// <summary>Provides a general description of what the web application does.</summary>
    [JsonPropertyName("description")]
    public string Description { get; set; }

    /// <summary>.</summary>
    [JsonPropertyName("iarc_rating_id")]
    public string IarcRatingId { get; set; }

    /// <summary>.</summary>
    [JsonPropertyName("categories")]
    public IEnumerable<string> Categories { get; set; }

    /// <summary>Specifies the primary text direction for the name, short_name, and description members.
    /// Together with the lang member, it can help provide the correct display of right-to-left languages.</summary>
    [JsonPropertyName("dir")]
    public string Dir { get; set; }

    /// <summary>Specifies the primary language for the values in the name and short_name members. This value is a string containing a single language tag.</summary>
    [JsonPropertyName("lang")]
    public string Lang { get; set; }

    /// <summary>If you don't provide a start_url, the current page is used, which is unlikely to be what your users want.</summary>
    [JsonPropertyName("start_url")]
    public string StartUrl { get; set; }

    /// <summary>A list of icons.</summary>
    [JsonPropertyName("icons")]
    public IEnumerable<Icon> Icons { get; set; }

    /// <summary>An array of objects. Each object represents a screenshot of the web app in a common usage scenario.</summary>
    [JsonPropertyName("screenshots")]
    public IEnumerable<Screenshot> Screenshots { get; set; }

    /// <summary>An array of objects. Each object represents a key task or page in the web app.</summary>
    [JsonPropertyName("shortcuts")]
    public IEnumerable<Shortcut> Shortcuts { get; set; }

    /// <summary>A hex color value.</summary>
    [JsonPropertyName("background_color")]
    public string BackgroundColor { get; set; }

    /// <summary>A hex color value.</summary>
    [JsonPropertyName("theme_color")]
    public string ThemeColor { get; set; }

    /// <summary>Defines the developer's preferred display mode for the web application.</summary>
    [JsonPropertyName("display")]
    public string Display { get; set; }

    /// <summary></summary>
    [JsonPropertyName("orientation")]
    public string Orientation { get; set; }

    /// <summary>specifies a boolean value that hints for the user agent to indicate to the user that the specified related applications are available, and recommended over the web application.</summary>
    [JsonPropertyName("prefer_related_applications")]
    public bool PreferRelatedApplications { get; set; }

    /// <summary>Specifies an array of "application objects" representing native applications that are installable by, or accessible to, the underlying platform.</summary>
    [JsonPropertyName("related_applications")]
    public IEnumerable<RelatedApplication> RelatedApplications { get; set; }

    /// <summary>Defines the navigation scope of this web application's application context.</summary>
    [JsonPropertyName("scope")]
    public string Scope { get; set; }

    /// <summary>
    /// Check if the manifest is valid
    /// </summary>
    public bool IsValid(out string error)
    {
        if (string.IsNullOrEmpty(this.Name) || string.IsNullOrEmpty(this.ShortName) || string.IsNullOrEmpty(this.StartUrl) || this.Icons == null)
        {
            error = $"The fields 'name', 'short_name', 'start_url' and 'icons' must be set  in {this.FileName}";
            return false;
        }

        if (this.Icons.All(i => i.Sizes?.Equals("512x512", StringComparison.OrdinalIgnoreCase) != true))
        {
            error = $"Missing icon in size 512x512 in {this.FileName}";
            return false;
        }

        if (this.Icons.All(i => i.Sizes?.Equals("192x192", StringComparison.OrdinalIgnoreCase) != true))
        {
            error = $"Missing icon in size 192x192 in {this.FileName}";
            return false;
        }

        error = "";
        return true;
    }
}