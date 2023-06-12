/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2023 Ingo Herbote
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

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Routing;

using RouteData = Microsoft.AspNetCore.Routing.RouteData;

[HtmlTargetElement("pager", TagStructure = TagStructure.NormalOrSelfClosing)]
public class PagerTagHelper : TagHelper, IPager, IHaveServiceLocator, IHaveLocalization
{
    /// <summary>
    ///   The localization.
    /// </summary>
    private ILocalization localization;

    /// <summary>
    ///   Gets Localization.
    /// </summary>
    public ILocalization Localization => this.localization ??= this.Get<ILocalization>();

    /// <summary>
    ///   Gets or sets ServiceLocator.
    /// </summary>
    public IServiceLocator ServiceLocator => BoardContext.Current.ServiceLocator;

    public BoardContext PageContext => BoardContext.Current;

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

    public override void Process(TagHelperContext context, TagHelperOutput output)
    {
        if (this.QueryName.IsNotSet())
        {
            this.QueryName = "p";
        }

        var routeData = this.Get<IHttpContextAccessor>().HttpContext.GetRouteData();

        int index;

        if (this.Get<IHttpContextAccessor>().HttpContext.Request.Query.ContainsKey(this.QueryName))
        {
            var query = this.Get<IHttpContextAccessor>().HttpContext.Request.Query[this.QueryName].FirstOrDefault()
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

        output.TagName = "nav";

        //output.Attributes.Add("class", "pagination");
        //output.Attributes.Add("role", "toolbar");

        var list = new TagBuilder("ul");

        list.AddCssClass("pagination");

        var listItem = new TagBuilder("li");

        listItem.AddCssClass("page-item disabled");

        var item = new TagBuilder("a");

        item.AddCssClass("page-link");
        item.MergeAttribute("href", "#");
        item.MergeAttribute("tabindex", "-1");
        item.MergeAttribute("aria-disabled", "true");

        item.InnerHtml.AppendHtml(this.Get<IHtmlHelper>().Icon("copy"));
        item.InnerHtml.Append($"{this.PageCount():N0} {this.GetText("COMMON", "PAGES")}");

        listItem.InnerHtml.AppendHtml(item);

        list.InnerHtml.AppendHtml(listItem);

        list.InnerHtml.AppendHtml(this.OutputLinks(routeData));

        output.Content.AppendHtml(list);
    }

    /// <summary>
    /// The output links.
    /// </summary>
    /// <param name="routeData"></param>
    private IHtmlContent OutputLinks(RouteData routeData)
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
            var listItem = new TagBuilder("li");
            listItem.AddCssClass("page-item");

            var link = new TagBuilder("a");

            link.AddCssClass("page-link");

            link.MergeAttribute("role", "button");
            link.MergeAttribute("data-bs-toggle", "tooltip");
            link.MergeAttribute("title", this.GetText("GOTOFIRSTPAGE_TT"));
            link.MergeAttribute("href", this.GetLinkUrl(query, 1));

            link.InnerHtml.AppendHtml(this.Get<IHtmlHelper>().Icon("angle-double-left"));

            listItem.InnerHtml.AppendHtml(link);

            content.AppendHtml(listItem);
        }

        if (this.CurrentPageIndex > start)
        {
            var listItem = new TagBuilder("li");
            listItem.AddCssClass("page-item");

            var link = new TagBuilder("a");

            link.AddCssClass("page-link");

            link.MergeAttribute("role", "button");
            link.MergeAttribute("data-bs-toggle", "tooltip");
            link.MergeAttribute("title", this.GetText("GOTOPREVPAGE_TT"));
            link.MergeAttribute("href", this.GetLinkUrl(query, this.CurrentPageIndex));

            link.InnerHtml.AppendHtml(this.Get<IHtmlHelper>().Icon("angle-left"));

            listItem.InnerHtml.AppendHtml(link);

            content.AppendHtml(listItem);
        }

        for (var i = start; i < end; i++)
        {
            var page = (i + 1).ToString();

            var listItem = new TagBuilder("li");
            listItem.AddCssClass(i == this.CurrentPageIndex ? "page-item active" : "page-item");

            var link = new TagBuilder("a");

            link.AddCssClass("page-link");

            link.MergeAttribute("role", "button");
            link.MergeAttribute("data-bs-toggle", "tooltip");
            link.MergeAttribute("title", $"{this.GetText("GOTOPAGE_HEADER")}{page}");
            link.MergeAttribute("href", this.GetLinkUrl(query, i + 1));

            link.InnerHtml.Append(page);

            listItem.InnerHtml.AppendHtml(link);

            content.AppendHtml(listItem);
        }

        if (this.CurrentPageIndex < this.PageCount() - 1)
        {
            var listItem = new TagBuilder("li");
            listItem.AddCssClass("page-item");

            var link = new TagBuilder("a");

            link.AddCssClass("page-link");

            link.MergeAttribute("role", "button");
            link.MergeAttribute("data-bs-toggle", "tooltip");
            link.MergeAttribute("title", this.GetText("GOTONEXTPAGE_TT"));
            link.MergeAttribute("href", this.GetLinkUrl(query, this.CurrentPageIndex + 2));

            link.InnerHtml.AppendHtml(this.Get<IHtmlHelper>().Icon("angle-right"));

            listItem.InnerHtml.AppendHtml(link);

            content.AppendHtml(listItem);
        }

        if (end >= this.PageCount())
        {
            return content;
        }

        var listItemNext = new TagBuilder("li");
        listItemNext.AddCssClass("page-item");

        var linkGoToNext = new TagBuilder("a");

        linkGoToNext.AddCssClass("page-link");

        linkGoToNext.MergeAttribute("role", "button");
        linkGoToNext.MergeAttribute("data-bs-toggle", "tooltip");
        linkGoToNext.MergeAttribute("title", this.GetText("GOTONEXTPAGE_TT"));
        linkGoToNext.MergeAttribute("href", this.GetLinkUrl(query, this.PageCount()));

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
    private string GetLinkUrl(IDictionary<string, string> query, int page)
    {
        if (query.ContainsKey(this.QueryName))
        {
            query[this.QueryName] = page.ToString();
        }
        else
        {
            query.Add(this.QueryName, page.ToString());
        }

        string url;
        switch (this.PageContext.CurrentForumPage.PageName)
        {
            case ForumPages.Topics:
                url = page > 1
                    ? this.Get<LinkBuilder>()
                        .GetLink(ForumPages.Topics,
                            new {f = this.PageContext.PageForumID, p = page, name = this.PageContext.PageForum.Name})
                    : this.Get<LinkBuilder>()
                        .GetLink(ForumPages.Topics,
                            new {f = this.PageContext.PageForumID, name = this.PageContext.PageForum.Name});
                break;
            case ForumPages.Posts:
                url = this.Get<LinkBuilder>()
                    .GetLink(ForumPages.Posts,
                        new {t = this.PageContext.PageTopicID, p = page, name = this.PageContext.PageTopic.TopicName});
                break;
            default:
                url = this.Get<LinkBuilder>()
                    .GetLink(this.PageContext.CurrentForumPage.PageName, query)
                    .Replace("p=", $"{this.QueryName}=");
                break;
        }

        return url;
    }
}