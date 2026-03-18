using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

using System;

using Microsoft.Extensions.Hosting;

namespace WebEssentials.AspNetCore.Pwa;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Razor.TagHelpers.TagHelperComponent" />
internal class ServiceWorkerTagHelperComponent : TagHelperComponent
{
    /// <summary>
    /// The script
    /// </summary>
    private readonly string _script;

    /// <summary>
    /// The env
    /// </summary>
    private readonly IWebHostEnvironment _env;

    /// <summary>
    /// The accessor
    /// </summary>
    private readonly IHttpContextAccessor _accessor;

    /// <summary>
    /// The options
    /// </summary>
    private readonly PwaOptions _options;

    /// <summary>
    /// Initializes a new instance of the <see cref="ServiceWorkerTagHelperComponent"/> class.
    /// </summary>
    /// <param name="env">The env.</param>
    /// <param name="accessor">The accessor.</param>
    /// <param name="options">The options.</param>
    public ServiceWorkerTagHelperComponent(IWebHostEnvironment env, IHttpContextAccessor accessor, PwaOptions options)
    {
        this._env = env;
        this._accessor = accessor;
        this._options = options;

        this._script =
            $"\r\n\t<script{(this._options.EnableCspNonce ? Constants.CspNonce : string.Empty)}>'serviceWorker'in navigator&&navigator.serviceWorker.register('{options.BaseRoute}{Constants.ServiceworkerRoute}', {{ scope: '{options.BaseRoute}' }})</script>";
    }

    /// <inheritdoc />
    public override int Order => -1;

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!this._options.RegisterServiceWorker)
        {
            return;
        }

        if (!string.Equals(context.TagName, "body", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if ((this._options.AllowHttp || this._accessor.HttpContext.Request.IsHttps) || this._env.IsDevelopment())
        {
            output.PostContent.AppendHtml(this._script);
        }
    }
}