using System;

using Microsoft.Extensions.Configuration;

namespace WebEssentials.AspNetCore.Pwa;

/// <summary>
/// Options for the service worker.
/// </summary>
public class PwaOptions
{
    /// <summary>
    /// Creates a new default instance of the options.
    /// </summary>
    public PwaOptions()
    {
        this.CacheId = Constants.DefaultCacheId;
        this.Strategy = ServiceWorkerStrategy.CustomStrategy;
        this.RoutesToPreCache = "";
        this.BaseRoute = "";
        this.OfflineRoute = Constants.Offlineroute;
        this.RegisterServiceWorker = true;
        this.RegisterWebmanifest = true;
        this.EnableCspNonce = false;
        this.ServiceWorkerCacheControlMaxAge = 60 * 60 * 24 * 30;    // 30 days
        this.WebManifestCacheControlMaxAge = 60 * 60 * 24 * 30;      // 30 days
        this.CustomServiceWorkerStrategyFileName = Constants.CustomServiceworkerFileName;
        this.RoutesToIgnore = "";
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="PwaOptions"/> class.
    /// </summary>
    /// <param name="config">The configuration.</param>
    internal PwaOptions(IConfiguration config)
        : this()
    {
        this.CacheId = config["pwa:cacheId"] ?? this.CacheId;
        this.RoutesToPreCache = config["pwa:routesToPreCache"] ?? this.RoutesToPreCache;
        this.BaseRoute = config["pwa:baseRoute"] ?? this.BaseRoute;
        this.OfflineRoute = config["pwa:offlineRoute"] ?? this.OfflineRoute;
        this.RoutesToIgnore = config["pwa:routesToIgnore"] ?? this.RoutesToIgnore;
        this.CustomServiceWorkerStrategyFileName =
            config["pwa:customServiceWorkerFileName"] ?? this.CustomServiceWorkerStrategyFileName;

        if (bool.TryParse(config["pwa:registerServiceWorker"] ?? "true", out var register))
        {
            this.RegisterServiceWorker = register;
        }

        if (bool.TryParse(config["pwa:registerWebmanifest"] ?? "true", out var manifest))
        {
            this.RegisterWebmanifest = manifest;
        }

        if (bool.TryParse(config["pwa:EnableCspNonce"] ?? "true", out var enableCspNonce))
        {
            this.EnableCspNonce = enableCspNonce;
        }

        if (Enum.TryParse(config["pwa:strategy"] ?? "cacheFirstSafe", true, out ServiceWorkerStrategy mode))
        {
            this.Strategy = mode;
        }

        if (int.TryParse(config["pwa:ServiceWorkerCacheControlMaxAge"], out var serviceWorkerCacheControlMaxAge))
        {
            this.ServiceWorkerCacheControlMaxAge = serviceWorkerCacheControlMaxAge;
        }

        if (int.TryParse(config["pwa:WebManifestCacheControlMaxAge"], out var webManifestCacheControlMaxAge))
        {
            this.WebManifestCacheControlMaxAge = webManifestCacheControlMaxAge;
        }
    }

    /// <summary>
    /// The cache identifier of the service worker (can be any string).
    /// Change this property to force the service worker to reload in browsers.
    /// </summary>
    public string CacheId { get; set; }

    /// <summary>
    /// Selects one of the predefined service worker types.
    /// </summary>
    public ServiceWorkerStrategy Strategy { get; set; }

    /// <summary>
    /// A comma separated list of routes to pre-cache when service worker installs in the browser.
    /// </summary>
    public string RoutesToPreCache { get; set; }

    /// <summary>
    /// The base route to the application.
    /// </summary>
    public string BaseRoute { get; set; }

    /// <summary>
    /// The route to the page to show when offline.
    /// </summary>
    public string OfflineRoute { get; set; }

    /// <summary>
    /// Determines if a script that registers the service worker should be injected
    /// into the bottom of the HTML page.
    /// </summary>
    public bool RegisterServiceWorker { get; set; }

    /// <summary>
    /// Determines if a meta tag that points to the web manifest should be inserted
    /// at the end of the head element.
    /// </summary>
    public bool RegisterWebmanifest { get; set; }

    /// <summary>
    /// Determines the value of the ServiceWorker CacheControl header Max-Age (in seconds)
    /// </summary>
    public int ServiceWorkerCacheControlMaxAge { get; set; }

    /// <summary>
    /// Gets or sets the web manifest cache control maximum age.
    /// </summary>
    /// <value>
    /// The web manifest cache control maximum age.
    /// </value>
    public int WebManifestCacheControlMaxAge { get; set; }

    /// <summary>
    /// Determines whether a CSP nonce will be added via NWebSec
    /// </summary>
    public bool EnableCspNonce { get; set; }

    /// <summary>
    /// Generate code even on HTTP connection. Necessary for SSL offloading.
    /// </summary>
    public bool AllowHttp { get; set; }

    /// <summary>
    /// The file name of the Custom ServiceWorker Strategy
    /// </summary>
    public string CustomServiceWorkerStrategyFileName { get; set; }

    /// <summary>
    /// A comma separated list of routes to ignore when implementing a CustomServiceworker.
    /// </summary>
    public string RoutesToIgnore { get; set; }
}