using System.IO;
using System.Text.Json;
using System.Text.RegularExpressions;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Caching.Memory;

namespace WebEssentials.AspNetCore.Pwa;

internal class WebManifestCache
{
    private readonly IWebHostEnvironment _env;
    private readonly MemoryCache _cache;
    private readonly string _fileName;

    public WebManifestCache(IWebHostEnvironment env, string fileName)
    {
        this._env = env;
        this._fileName = fileName;
        this._cache = new MemoryCache(new MemoryCacheOptions());
    }

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