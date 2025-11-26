using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Razor.TagHelpers;

using System;

using Microsoft.Extensions.Hosting;

namespace WebEssentials.AspNetCore.Pwa;

internal class ServiceWorkerTagHelperComponent : TagHelperComponent
{
    private readonly string _script;

    private readonly IWebHostEnvironment _env;
    private readonly IHttpContextAccessor _accessor;
    private readonly PwaOptions _options;

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