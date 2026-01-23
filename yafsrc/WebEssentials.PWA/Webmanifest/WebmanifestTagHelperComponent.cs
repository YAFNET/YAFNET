using System;

using HtmlProperties;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebEssentials.AspNetCore.Pwa;

/// <summary>
/// 
/// </summary>
/// <seealso cref="Microsoft.AspNetCore.Razor.TagHelpers.TagHelperComponent" />
internal class WebmanifestTagHelperComponent : TagHelperComponent
{
    private readonly PwaOptions _options;
    private readonly IServiceProvider _serviceProvider;

    /// <summary>
    /// Initializes a new instance of the <see cref="WebmanifestTagHelperComponent"/> class.
    /// </summary>
    /// <param name="options">The options.</param>
    /// <param name="serviceProvider">The service provider.</param>
    public WebmanifestTagHelperComponent(PwaOptions options, IServiceProvider serviceProvider)
    {
        this._options = options;
        this._serviceProvider = serviceProvider;
    }

    /// <inheritdoc />
    public override int Order => 100;

    /// <inheritdoc />
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (!this._options.RegisterWebmanifest)
        {
            return;
        }

        if (this._serviceProvider.GetService(typeof(WebManifest)) is not WebManifest manifest)
        {
            return;
        }

        if (!string.Equals(context.TagName, "head", StringComparison.OrdinalIgnoreCase))
        {
            return;
        }

        if (!string.IsNullOrEmpty(manifest.ThemeColor))
        {
            var themeColorMetaTag = new TagBuilder(HtmlTag.Meta);

            themeColorMetaTag.MergeAttribute(HtmlAttribute.Name, "theme-color");
            themeColorMetaTag.MergeAttribute(HtmlAttribute.Content, manifest.ThemeColor);

            themeColorMetaTag.TagRenderMode = TagRenderMode.SelfClosing;

            output.PostContent.AppendHtml(themeColorMetaTag);
        }

        output.PostContent.AppendHtml(this.CreateLink("manifest",
            $"{this._options.BaseRoute}{Constants.WebManifestRoute}"));

        var mobileMetaTag = new TagBuilder(HtmlTag.Meta);

        mobileMetaTag.MergeAttribute(HtmlAttribute.Name, "mobile-web-app-capable");
        mobileMetaTag.MergeAttribute(HtmlAttribute.Content, "yes");

        mobileMetaTag.TagRenderMode = TagRenderMode.SelfClosing;

        output.PostContent.AppendHtml(mobileMetaTag);

        const string appleStartupImage = "apple-touch-startup-image";

        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2048-2732.webp",
            "(device-width: 1024px) and (device-height: 1366px) and (-webkit-device-pixel-ratio: 2) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink("appleStartupImage",
            $"{this._options.BaseRoute}assets/apple-splash-2732-2048.webp",
            "(device-width: 1024px) and (device-height: 1366px) and (-webkit-device-pixel-ratio: 2) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1668-2388.webp",
            "(device-width: 834px) and (device-height: 1194px) and (-webkit-device-pixel-ratio: 2) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2388-1668.webp",
            "(device-width: 834px) and (device-height: 1194px) and (-webkit-device-pixel-ratio: 2) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1536-2048.webp",
            "(device-width: 768px) and (device-height: 1024px) and (-webkit-device-pixel-ratio: 2) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2048-1536.webp",
            "(device-width: 768px) and (device-height: 1024px) and (-webkit-device-pixel-ratio: 2) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1640-2360.webp",
            "(device-width: 820px) and (device-height: 1180px) and (-webkit-device-pixel-ratio: 2) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2360-1640.webp",
            "(device-width: 820px) and (device-height: 1180px) and (-webkit-device-pixel-ratio: 2) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1668-2224.webp",
            "(device-width: 834px) and (device-height: 1112px) and (-webkit-device-pixel-ratio: 2) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2224-1668.webp",
            "(device-width: 834px) and (device-height: 1112px) and (-webkit-device-pixel-ratio: 2) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1620-2160.webp",
            "(device-width: 810px) and (device-height: 1080px) and (-webkit-device-pixel-ratio: 2) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2160-1620.webp",
            "(device-width: 810px) and (device-height: 1080px) and (-webkit-device-pixel-ratio: 2) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1488-2266.webp",
            "(device-width: 744px) and (device-height: 1133px) and (-webkit-device-pixel-ratio: 2) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2266-1488.webp",
            "(device-width: 744px) and (device-height: 1133px) and (-webkit-device-pixel-ratio: 2) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1320-2868.webp",
            "(device-width: 440px) and (device-height: 956px) and (-webkit-device-pixel-ratio: 3) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2868-1320.webp",
            "(device-width: 440px) and (device-height: 956px) and (-webkit-device-pixel-ratio: 3) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1206-2622.webp",
            "(device-width: 402px) and (device-height: 874px) and (-webkit-device-pixel-ratio: 3) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2622-1206.webp",
            "(device-width: 402px) and (device-height: 874px) and (-webkit-device-pixel-ratio: 3) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1260-2736.webp",
            "(device-width: 420px) and (device-height: 912px) and (-webkit-device-pixel-ratio: 3) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2736-1260.webp",
            "(device-width: 420px) and (device-height: 912px) and (-webkit-device-pixel-ratio: 3) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1290-2796.webp",
            "(device-width: 430px) and (device-height: 932px) and (-webkit-device-pixel-ratio: 3) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2796-1290.webp",
            "(device-width: 430px) and (device-height: 932px) and (-webkit-device-pixel-ratio: 3) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1179-2556.webp",
            "(device-width: 393px) and (device-height: 852px) and (-webkit-device-pixel-ratio: 3) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2556-1179.webp",
            "(device-width: 393px) and (device-height: 852px) and (-webkit-device-pixel-ratio: 3) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1170-2532.webp",
            "(device-width: 390px) and (device-height: 844px) and (-webkit-device-pixel-ratio: 3) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2532-1170.webp",
            "(device-width: 390px) and (device-height: 844px) and (-webkit-device-pixel-ratio: 3) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1284-2778.webp",
            "(device-width: 428px) and (device-height: 926px) and (-webkit-device-pixel-ratio: 3) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2778-1284.webp",
            "(device-width: 428px) and (device-height: 926px) and (-webkit-device-pixel-ratio: 3) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1125-2436.webp",
            "(device-width: 375px) and (device-height: 812px) and (-webkit-device-pixel-ratio: 3) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2436-1125.webp",
            "(device-width: 375px) and (device-height: 812px) and (-webkit-device-pixel-ratio: 3) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1242-2688.webp",
            "(device-width: 414px) and (device-height: 896px) and (-webkit-device-pixel-ratio: 3) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2688-1242.webp",
            "(device-width: 414px) and (device-height: 896px) and (-webkit-device-pixel-ratio: 3) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-828-1792.webp",
            "(device-width: 414px) and (device-height: 896px) and (-webkit-device-pixel-ratio: 2) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1792-828.webp",
            "(device-width: 414px) and (device-height: 896px) and (-webkit-device-pixel-ratio: 2) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1242-2208.webp",
            "(device-width: 414px) and (device-height: 736px) and (-webkit-device-pixel-ratio: 3) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-2208-1242.webp",
            "(device-width: 414px) and (device-height: 736px) and (-webkit-device-pixel-ratio: 3) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-750-1334.webp",
            "(device-width: 375px) and (device-height: 667px) and (-webkit-device-pixel-ratio: 2) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1334-750.webp",
            "(device-width: 375px) and (device-height: 667px) and (-webkit-device-pixel-ratio: 2) and (orientation: landscape)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-640-1136.webp",
            "(device-width: 320px) and (device-height: 568px) and (-webkit-device-pixel-ratio: 2) and (orientation: portrait)"));
        output.PostContent.AppendHtml(this.CreateLink(appleStartupImage,
            $"{this._options.BaseRoute}assets/apple-splash-1136-640.webp",
            "(device-width: 320px) and (device-height: 568px) and (-webkit-device-pixel-ratio: 2) and (orientation: landscape)"));

        output.PostContent.AppendHtml(this.CreateLink("apple-touch-icon",
            $"{this._options.BaseRoute}assets/apple-icon-180.webp"));
        output.PostContent.AppendHtml(this.CreateLink("shortcut icon", $"{this._options.BaseRoute}assets/favicon.ico"));
    }

    /// <summary>
    /// Creates the link.
    /// </summary>
    /// <param name="rel">The relative.</param>
    /// <param name="href">The href.</param>
    /// <param name="media">The media query.</param>
    /// <returns></returns>
    private TagBuilder CreateLink(string rel, string href, string media = null)
    {
        var link = new TagBuilder(HtmlTag.Link);

        link.MergeAttribute(HtmlAttribute.Rel, rel);
        link.MergeAttribute(HtmlAttribute.Href, href);

        if (!string.IsNullOrEmpty(media))
        {
            link.MergeAttribute(HtmlAttribute.Media, media);
        }

        link.TagRenderMode = TagRenderMode.SelfClosing;

        return link;
    }
}