using System;

using HtmlProperties;

using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Razor.TagHelpers;

namespace WebEssentials.AspNetCore.Pwa
{
    internal class WebmanifestTagHelperComponent : TagHelperComponent
    {
        private readonly PwaOptions _options;
        private readonly IServiceProvider _serviceProvider;

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


            output.PostContent.AppendHtml(this.CreateLink("manifest", $"{this._options.BaseRoute}{Constants.WebManifestRoute}"));

            var mobileMetaTag = new TagBuilder(HtmlTag.Meta);

            mobileMetaTag.MergeAttribute(HtmlAttribute.Name, "mobile-web-app-capable");
            mobileMetaTag.MergeAttribute(HtmlAttribute.Content, "yes");

            mobileMetaTag.TagRenderMode = TagRenderMode.SelfClosing;

            output.PostContent.AppendHtml(mobileMetaTag);

            output.PostContent.AppendHtml(this.CreateLink("apple-touch-icon", "~/assets/apple-icon-180.webp"));
        }

        private TagBuilder CreateLink(string rel, string href)
        {
            var link = new TagBuilder(HtmlTag.Link);

            link.MergeAttribute(HtmlAttribute.Rel, rel);
            link.MergeAttribute(HtmlAttribute.Href, href);

            link.TagRenderMode = TagRenderMode.SelfClosing;

            return link;
        }
    }
}
