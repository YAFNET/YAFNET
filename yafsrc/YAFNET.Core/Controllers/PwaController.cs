using System;
using System.IO;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

using Microsoft.Net.Http.Headers;

using WebEssentials.AspNetCore.Pwa;

using Constants = WebEssentials.AspNetCore.Pwa.Constants;

namespace YAF.Core.Controllers;

/// <summary>
/// A controller for manifest.webmanifest, serviceworker.js and offline.html
/// </summary>
[Route("/")]
public class PwaController : Controller
{
    private readonly PwaOptions _options;
    private readonly RetrieveCustomServiceworker _customServiceworker;
    private readonly JsonSerializerOptions _jsonOptions;

    /// <summary>
    /// Creates an instance of the controller.
    /// </summary>
    public PwaController(PwaOptions options, RetrieveCustomServiceworker customServiceworker)
    {
        this._options = options;
        this._customServiceworker = customServiceworker;
        this._jsonOptions = new JsonSerializerOptions { DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
    }

    /// <summary>
    /// Serves a service worker based on the provided settings.
    /// </summary>
    [Route(Constants.ServiceworkerRoute)]
    [HttpGet]
    public Task<IActionResult> ServiceWorkerAsync()
    {
        this.Response.ContentType = "application/javascript; charset=utf-8";
        this.Response.Headers[HeaderNames.CacheControl] = $"max-age={this._options.ServiceWorkerCacheControlMaxAge}";

        var js = this._customServiceworker.GetCustomServiceworker(this._options.CustomServiceWorkerStrategyFileName);
        return Task.FromResult<IActionResult>(this.Content(this.InsertStrategyOptions(js)));
    }

    private string InsertStrategyOptions(string javascriptString)
    {
        return javascriptString
            .Replace("{version}", $"{this._options.CacheId}::{this._options.Strategy}")
            .Replace("{routes}", string.Join(",", this._options.RoutesToPreCache.Split([','], StringSplitOptions.RemoveEmptyEntries).Select(r =>
                $"'{r.Trim()}'")))
            .Replace("{offlineRoute}", this._options.BaseRoute + this._options.OfflineRoute)
            .Replace("{offlineHtml}", this._options.BaseRoute + this._options.OfflineRoute)
            .Replace("{ignoreRoutes}", string.Join(",", this._options.RoutesToIgnore.Split([','], StringSplitOptions.RemoveEmptyEntries).Select(r =>
                $"'{r.Trim()}'")));
    }

    /// <summary>
    /// Serves the offline.html file
    /// </summary>
    [Route(Constants.Offlineroute)]
    [HttpGet]
    public async Task<IActionResult> OfflineAsync()
    {
        this.Response.ContentType = "text/html";

        var assembly = typeof(PwaOptions).Assembly;
        var resourceStream = assembly.GetManifestResourceStream("WebEssentials.AspNetCore.Pwa.ServiceWorker.Files.offline.html");

        using var reader = new StreamReader(resourceStream);
        return this.Content(await reader.ReadToEndAsync());
    }

    /// <summary>
    /// Serves the manifest.json file
    /// </summary>
    [Route(Constants.WebManifestRoute)]
    [HttpGet]
    public IActionResult WebManifest([FromServices] WebManifest wm)
    {
        if (wm == null)
        {
            return this.NotFound();
        }

        this.Response.ContentType = "application/manifest+json; charset=utf-8";

        this.Response.Headers[HeaderNames.CacheControl] = $"max-age={this._options.WebManifestCacheControlMaxAge}";

        wm.Name = BoardContext.Current.BoardSettings.Name;
        wm.Description = BoardContext.Current.BoardSettings.Description;

        return this.Content(JsonSerializer.Serialize(wm, this._jsonOptions));
    }
}