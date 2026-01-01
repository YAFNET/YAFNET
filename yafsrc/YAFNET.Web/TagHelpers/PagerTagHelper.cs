/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2026 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Web.TagHelpers;

using Microsoft.AspNetCore.Mvc.ViewFeatures;
using Microsoft.AspNetCore.Routing;

using RouteData = Microsoft.AspNetCore.Routing.RouteData;

/// <summary>
/// Class PagerTagHelper.
/// Implements the <see cref="TagHelper" />
/// Implements the <see cref="IPager" />
/// Implements the <see cref="IHaveServiceLocator" />
/// Implements the <see cref="IHaveLocalization" />
/// </summary>
/// <seealso cref="TagHelper" />
/// <seealso cref="IPager" />
/// <seealso cref="IHaveServiceLocator" />
/// <seealso cref="IHaveLocalization" />
[HtmlTargetElement("pager", TagStructure = TagStructure.NormalOrSelfClosing)]
public class PagerTagHelper : TagHelper, IPager, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => field ??= this.Get<ILocalization>();

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    /// <summary>
    /// Gets the page context.
    /// </summary>
    /// <value>The page context.</value>
    public BoardContext PageContext => BoardContext.Current;

    /// <summary>
    /// Gets or sets the view context.
    /// </summary>
    /// <value>The view context.</value>
    [ViewContext]
    public ViewContext ViewContext { get; set; }

    /// <summary>
    /// Gets or sets the name of the query.
    /// </summary>
    /// <value>The name of the query.</value>
    [HtmlAttributeName("query-name")]
    public string QueryName { get; set; }

    /// <summary>
    ///   Gets or sets Count.
    /// </summary>
    [HtmlAttributeName("count")]
    public int Count { get; set; }

    /// <summary>
    ///   Gets or sets CurrentPageIndex.
    /// </summary>
    [HtmlAttributeName("current-page-index")]
    public int CurrentPageIndex { get; set; }

    /// <summary>
    ///   Gets or sets PageSize.
    /// </summary>
    [HtmlAttributeName("page-size")]
    public int PageSize { get; set; }

    /// <summary>
    /// Gets or sets a value indicating whether to use Submit buttons or links.
    /// </summary>
    [HtmlAttributeName("use-submit")] public bool UseSubmit { get; set; } = true;

    /// <summary>
    /// Synchronously executes the <see cref="T:Microsoft.AspNetCore.Razor.TagHelpers.TagHelper" /> with the given <paramref name="context" /> and
    /// <paramref name="output" />.
    /// </summary>
    /// <param name="context">Contains information associated with the current HTML tag.</param>
    /// <param name="output">A stateful HTML element used to generate an HTML tag.</param>
    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (this.QueryName.IsNotSet())
        {
            this.QueryName = "p";
        }

        var routeData = this.ViewContext.HttpContext.GetRouteData();

        int index;

        if (this.ViewContext.HttpContext.Request.Query.ContainsKey(this.QueryName))
        {
            var query = this.ViewContext.HttpContext.Request.Query[this.QueryName].FirstOrDefault()
            .ToType<int>();

            index = query > 0 ? query - 1 : 0;
        }
        else
        {
            index = routeData.Values[this.QueryName].ToTypeOrDefault(0) - 1;
        }

        if (index == -1)
        {
            index = 0;
        }

        this.CurrentPageIndex = index;

        if (this.PageCount() < 2)
        {
            return;
        }

        output.TagName = HtmlTag.Nav;

        var list = new TagBuilder(HtmlTag.Ul);

        list.AddCssClass("pagination");

        var listItem = new TagBuilder(HtmlTag.Li);

        listItem.AddCssClass("page-item disabled");

        var item = new TagBuilder(HtmlTag.Button);

        item.AddCssClass("page-link");
        item.MergeAttribute(HtmlAttribute.Href, "#");
        item.MergeAttribute(HtmlAttribute.Tabindex, "-1");
        item.MergeAttribute("aria-disabled", "true");
        item.MergeAttribute(HtmlAttribute.Title, $"{this.PageCount():N0} {this.GetText("COMMON", "PAGES")}");

        item.InnerHtml.AppendHtml(this.Get<IHtmlHelper>().Icon("file"));

        item.InnerHtml.AppendHtml($"""{this.PageCount():N0}<span class="d-none d-md-inline-block ms-1">{this.GetText("COMMON", "PAGES")}</span>""");

        listItem.InnerHtml.AppendHtml(item);

        list.InnerHtml.AppendHtml(listItem);

        list.InnerHtml.AppendHtml(this.OutputLinks(routeData));

        output.Content.AppendHtml(list);
    }

    /// <summary>
    /// The output links.
    /// </summary>
    /// <param name="routeData"></param>
    private HtmlContentBuilder OutputLinks(RouteData routeData)
    {
        var query = routeData.Values.ToDictionary(q => q.Key, q => q.Value.ToString());

        var content = new HtmlContentBuilder();

        var start = this.CurrentPageIndex - 2;
        var end = this.CurrentPageIndex + 3;

        if (start < 0)
        {
            start = 0;
        }

        if (end > this.PageCount())
        {
            end = this.PageCount();
        }

        if (start > 0)
        {
            var listItem = new TagBuilder(HtmlTag.Li);
            listItem.AddCssClass("page-item");

            var link = new TagBuilder(this.UseSubmit ? HtmlTag.Button : HtmlTag.A);

            link.AddCssClass("page-link");

            if (this.UseSubmit)
            {
                link.MergeAttribute(HtmlAttribute.Type, "submit");
            }

            link.MergeAttribute("data-bs-toggle", "tooltip");
            link.MergeAttribute(HtmlAttribute.Title, this.GetText("GOTOFIRSTPAGE_TT"));
            link.MergeAttribute(this.UseSubmit ? HtmlAttribute.Formaction: HtmlAttribute.Href, this.GetLinkUrl(query, 1));

            link.InnerHtml.AppendHtml(this.Get<IHtmlHelper>().Icon("angle-double-left"));

            listItem.InnerHtml.AppendHtml(link);

            content.AppendHtml(listItem);
        }

        if (this.CurrentPageIndex > start)
        {
            var listItem = new TagBuilder(HtmlTag.Li);
            listItem.AddCssClass("page-item");

            var link = new TagBuilder(this.UseSubmit ? HtmlTag.Button : HtmlTag.A);

            link.AddCssClass("page-link");

            if (this.UseSubmit)
            {
                link.MergeAttribute(HtmlAttribute.Type, "submit");
            }

            link.MergeAttribute("data-bs-toggle", "tooltip");
            link.MergeAttribute(HtmlAttribute.Title, this.GetText("GOTOPREVPAGE_TT"));
            link.MergeAttribute(this.UseSubmit ? HtmlAttribute.Formaction : HtmlAttribute.Href, this.GetLinkUrl(query, this.CurrentPageIndex));

            link.InnerHtml.AppendHtml(this.Get<IHtmlHelper>().Icon("angle-left"));

            listItem.InnerHtml.AppendHtml(link);

            content.AppendHtml(listItem);
        }

        for (var i = start; i < end; i++)
        {
            var page = (i + 1).ToString();

            var listItem = new TagBuilder(HtmlTag.Li);
            listItem.AddCssClass(i == this.CurrentPageIndex ? "page-item active" : "page-item");

            var link = new TagBuilder(this.UseSubmit ? HtmlTag.Button : HtmlTag.A);

            link.AddCssClass("page-link");

            if (this.UseSubmit)
            {
                link.MergeAttribute(HtmlAttribute.Type, "submit");
            }

            link.MergeAttribute("data-bs-toggle", "tooltip");
            link.MergeAttribute(HtmlAttribute.Title, $"{this.GetText("GOTOPAGE_HEADER")}{page}");
            link.MergeAttribute(this.UseSubmit ? HtmlAttribute.Formaction : HtmlAttribute.Href, this.GetLinkUrl(query, i + 1));

            link.InnerHtml.Append(page);

            listItem.InnerHtml.AppendHtml(link);

            content.AppendHtml(listItem);
        }

        if (this.CurrentPageIndex < this.PageCount() - 1)
        {
            var listItem = new TagBuilder(HtmlTag.Li);
            listItem.AddCssClass("page-item");

            var link = new TagBuilder(this.UseSubmit ? HtmlTag.Button : HtmlTag.A);

            link.AddCssClass("page-link");

            if (this.UseSubmit)
            {
                link.MergeAttribute(HtmlAttribute.Type, "submit");
            }

            link.MergeAttribute("data-bs-toggle", "tooltip");
            link.MergeAttribute(HtmlAttribute.Title, this.GetText("GOTONEXTPAGE_TT"));
            link.MergeAttribute(this.UseSubmit ? HtmlAttribute.Formaction : HtmlAttribute.Href, this.GetLinkUrl(query, this.CurrentPageIndex + 2));

            link.InnerHtml.AppendHtml(this.Get<IHtmlHelper>().Icon("angle-right"));

            listItem.InnerHtml.AppendHtml(link);

            content.AppendHtml(listItem);
        }

        if (end >= this.PageCount())
        {
            return content;
        }

        var listItemNext = new TagBuilder(HtmlTag.Li);
        listItemNext.AddCssClass("page-item");

        var linkGoToNext = new TagBuilder(this.UseSubmit ? HtmlTag.Button : HtmlTag.A);

        linkGoToNext.AddCssClass("page-link");

        if (this.UseSubmit)
        {
            linkGoToNext.MergeAttribute(HtmlAttribute.Type, "submit");
        }

        linkGoToNext.MergeAttribute("data-bs-toggle", "tooltip");
        linkGoToNext.MergeAttribute(HtmlAttribute.Title, this.GetText("GOTONEXTPAGE_TT"));
        linkGoToNext.MergeAttribute(this.UseSubmit ? HtmlAttribute.Formaction : HtmlAttribute.Href, this.GetLinkUrl(query, this.PageCount()));

        linkGoToNext.InnerHtml.AppendHtml(this.Get<IHtmlHelper>().Icon("angle-double-right"));

        listItemNext.InnerHtml.AppendHtml(linkGoToNext);

        content.AppendHtml(listItemNext);

        return content;
    }

    /// <summary>
    /// Gets the link URL.
    /// </summary>
    /// <returns>
    /// The get link url.
    /// </returns>
    private string GetLinkUrl(Dictionary<string, string> query, int page)
    {
        query[this.QueryName] = page.ToString();

        var url = this.PageContext.CurrentForumPage.PageName switch {
            ForumPages.Topics => page > 1
                ? this.Get<ILinkBuilder>()
                    .GetLink(ForumPages.Topics,
                        new { f = this.PageContext.PageForumID, p = page, name = this.PageContext.PageForum.Name })
                : this.Get<ILinkBuilder>()
                    .GetForumLink(this.PageContext.PageForum),
            ForumPages.Posts => this.Get<ILinkBuilder>()
                .GetLink(ForumPages.Posts,
                    new { t = this.PageContext.PageTopicID, p = page, name = this.PageContext.PageTopic.TopicName }),
            _ => this.Get<ILinkBuilder>().GetLink(this.PageContext.CurrentForumPage.PageName, query)
                .Replace("p=", $"{this.QueryName}=")
        };

        return url;
    }
}