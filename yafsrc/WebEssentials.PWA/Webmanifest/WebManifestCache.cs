using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace WebEssentials.AspNetCore.Pwa;

/// <summary>
/// 
/// </summary>
internal class WebManifestCache
{
    /// <summary>
    /// The env
    /// </summary>
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// The cache
    /// </summary>
    private readonly MemoryCache _cache;

    /// <summary>
    /// The file name
    /// </summary>
    private readonly string _fileName;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebManifestCache"/> class.
    /// </summary>
    /// <param name="env">The env.</param>
    /// <param name="fileName">Name of the file.</param>
    public WebManifestCache(IWebHostEnvironment env, string fileName)
    {
        this._env = env;
        this._fileName = fileName;
        this._cache = new MemoryCache(new MemoryCacheOptions());
    }

    /// <summary>
    /// Gets the manifest.
    /// </summary>
    /// <returns></returns>
    public WebManifest GetManifest()
    {
        return this._cache.GetOrCreate("webmanifest", (entry) =>
        {
            var file = this._env.WebRootFileProvider.GetFileInfo(this._fileName);
            entry.AddExpirationToken(this._env.WebRootFileProvider.Watch(this._fileName));

            var json = File.ReadAllText(file.PhysicalPath);

            var manifest = JsonSerializer.Deserialize<WebManifest>(json);
            manifest.FileName = this._fileName;
            manifest.RawJson = Regex.Replace(json, "(\"(?:[^\"\\\\]|\\\\.)*\")|\\s+", "$1", RegexOptions.NonBacktracking);

            return !manifest.IsValid(out var error) ? throw new JsonException(error) : manifest;
        });
    }
}