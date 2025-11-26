namespace WebEssentials.AspNetCore.Pwa;

/// <summary>
/// PWA related constants
/// </summary>
public static class Constants
{
    /// <summary>
    /// The serviceworker route
    /// </summary>
    public const string ServiceworkerRoute = "serviceworker";

    /// <summary>
    /// The custom serviceworker file name
    /// </summary>
    public const string CustomServiceworkerFileName = "customserviceworker.js";

    /// <summary>
    /// The offlineroute
    /// </summary>
    public const string Offlineroute = "offline.html";

    /// <summary>
    /// The default cache identifier
    /// </summary>
    public const string DefaultCacheId = "v1.0";

    /// <summary>
    /// The web manifest route
    /// </summary>
    public const string WebManifestRoute = "manifest.webmanifest";

    /// <summary>
    /// The web manifest file name
    /// </summary>
    public const string WebManifestFileName = "manifest.json";

    /// <summary>
    /// The CSP nonce attribute
    /// </summary>
    public const string CspNonce = " nws-csp-add-nonce='true'";
}