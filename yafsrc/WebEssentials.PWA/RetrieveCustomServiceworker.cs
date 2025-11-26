using System.IO;

using Microsoft.AspNetCore.Hosting;

namespace WebEssentials.AspNetCore.Pwa;

/// <summary>
/// A utility that can retrieve the contents of a CustomServiceworker strategy file
/// </summary>
public class RetrieveCustomServiceworker
{
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// Initializes a new instance of the <see cref="RetrieveCustomServiceworker"/> class.
    /// </summary>
    /// <param name="env">The env.</param>
    public RetrieveCustomServiceworker(IWebHostEnvironment env)
    {
        this._env = env;
    }

    /// <summary>
    /// Returns a <seealso cref="string"/> containing the contents of a Custom Serviceworker javascript file
    /// </summary>
    /// <returns></returns>
    public string GetCustomServiceworker(string fileName = "customserviceworker.js")
    {
        var file = this._env.WebRootFileProvider.GetFileInfo(fileName);
        return File.ReadAllText(file.PhysicalPath);
    }
}