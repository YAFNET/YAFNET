﻿/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2025 Ingo Herbote
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

namespace YAF.Web.Controls;

/// <summary>
/// Pager Control.
/// </summary>
public class Pager : BaseControl, IPostBackEventHandler, IPager
{
    /// <summary>
    ///   The ignore page index.
    /// </summary>
    private bool ignorePageIndex;

    /// <summary>
    ///   The page change.
    /// </summary>
    public event EventHandler PageChange;

    /// <summary>
    ///   Gets or sets Count.
    /// </summary>
    public int Count
    {
        get => this.ViewState["Count"].ToType<int?>() ?? 0;

        set => this.ViewState["Count"] = value;
    }

    /// <summary>
    ///   Gets or sets CurrentPageIndex.
    /// </summary>
    public int CurrentPageIndex
    {
        get => (this.ViewState["CurrentPageIndex"] ?? 0).ToType<int>();

        set => this.ViewState["CurrentPageIndex"] = value;
    }

    /// <summary>
    ///   Gets or sets LinkedPager.
    /// </summary>
    public string LinkedPager
    {
        get => (string)this.ViewState["LinkedPager"];

        set => this.ViewState["LinkedPager"] = value;
    }

    /// <summary>
    ///   Gets or sets PageSize.
    /// </summary>
    public int PageSize
    {
        get => (int?)this.ViewState["PageSize"] ?? 20;

        set => this.ViewState["PageSize"] = value;
    }

    /// <summary>
    ///   Gets or sets a value indicating whether UsePostBack.
    /// </summary>
    public bool UsePostBack { get; set; } = true;

    /// <summary>
    ///   Gets the Current Linked Pager.
    /// </summary>
    protected Pager CurrentLinkedPager
    {
        get
        {
            if (this.LinkedPager == null)
            {
                return null;
            }

            var linkedPager = this.Parent.FindControlAs<Pager>(this.LinkedPager) ?? throw new Exception($"Failed to link pager to '{this.LinkedPager}'.");
            return linkedPager;
        }
    }

    /// <summary>
    /// The raise post back event.
    /// </summary>
    /// <param name="eventArgument">
    /// The event argument.
    /// </param>
    public void RaisePostBackEvent(string eventArgument)
    {
        if (this.LinkedPager != null)
        {
            // raise post back event on the linked pager...
            this.CurrentLinkedPager.RaisePostBackEvent(eventArgument);
        }
        else if (this.PageChange != null)
        {
            this.CurrentPageIndex = int.Parse(eventArgument) - 1;
            this.ignorePageIndex = true;
            this.PageChange(this, EventArgs.Empty);
        }
    }

    /// <summary>
    /// Copies the pager settings.
    /// </summary>
    /// <param name="toPager">To pager.</param>
    protected void CopyPagerSettings(Pager toPager)
    {
        toPager.Count = this.Count;
        toPager.CurrentPageIndex = this.CurrentPageIndex;
        toPager.PageSize = this.PageSize;
    }

    /// <summary>
    /// Gets the page URL.
    /// </summary>
    /// <param name="page">The page.</param>
    /// <returns>
    /// The get page url.
    /// </returns>
    protected string GetPageUrl(int page)
    {
        var url = this.PageBoardContext.CurrentForumPage.PageType switch
            {
                ForumPages.Topics => page > 1
                                         ? this.Get<LinkBuilder>().GetLink(
                                             ForumPages.Topics,
                                             new { f = this.PageBoardContext.PageForumID, p = page, name = this.PageBoardContext.PageForum.Name })
                                         : this.Get<LinkBuilder>().GetLink(
                                             ForumPages.Topics,
                                             new { f = this.PageBoardContext.PageForumID, name = this.PageBoardContext.PageForum.Name }),
                ForumPages.Posts => this.Get<LinkBuilder>().GetLink(
                    ForumPages.Posts,
                    new { t = this.PageBoardContext.PageTopicID, p = page, name = this.PageBoardContext.PageTopic.TopicName }),
                _ => string.Empty
            };

        return url;
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
    override protected void OnInit(EventArgs e)
    {
        base.OnInit(e);

        if (!this.ignorePageIndex && this.Get<HttpRequestBase>().QueryString.Exists("p"))
        {
            // set a new page...
            this.CurrentPageIndex =
                this.Get<LinkBuilder>().StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p")) - 1;
        }
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The output.
    /// </param>
    override protected void Render(HtmlTextWriter writer)
    {
        if (this.LinkedPager != null)
        {
            // just copy the linked pager settings but still render in this function...
            this.CurrentLinkedPager.CopyPagerSettings(this);
        }

        if (this.PageCount() < 2)
        {
            return;
        }

        writer.Write(@"<div class=""btn-toolbar pagination"" role=""toolbar"">");

        writer.Write(@"<div class=""btn-group mb-1 d-none d-lg-inline-block me-2"" role=""group"">");

        writer.WriteLine(
            @"<button type=""button"" title=""{0}"" class=""btn btn-secondary disabled"" aria-disabled=""true"">
                          <i class=""fas fa-copy""></i>&nbsp;{1:N0} {2}
                      </button>",
            this.Get<ILocalization>().TransPage.IsSet()
                ? this.GetText("COMMON", "GOTOPAGE_HEADER")
                : "Go to page...",
            this.PageCount(),
            this.GetText("COMMON", "PAGES"));

        writer.Write(@"</div><div class=""btn-group mb-1"" role=""group"">");

        this.OutputLinks(writer, this.UsePostBack);

        writer.WriteLine("</div></div>");
    }

    /// <summary>
    /// Gets the link URL.
    /// </summary>
    /// <param name="pageNum">The page number.</param>
    /// <param name="postBack">The post back.</param>
    /// <returns>
    /// The get link url.
    /// </returns>
    private string GetLinkUrl(int pageNum, bool postBack)
    {
        return postBack
                   ? this.Page.ClientScript.GetPostBackClientHyperlink(this, pageNum.ToString())
                   : this.GetPageUrl(pageNum);
    }

    /// <summary>
    /// The output links.
    /// </summary>
    /// <param name="output">
    /// The output.
    /// </param>
    /// <param name="postBack">
    /// The post back.
    /// </param>
    private void OutputLinks(HtmlTextWriter output, bool postBack)
    {
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
            var link = new ThemeButton
                           {
                               NavigateUrl = this.GetLinkUrl(1, postBack),
                               Type = ButtonStyle.Secondary,
                               TitleLocalizedPage = "COMMON",
                               TitleLocalizedTag = "GOTOFIRSTPAGE_TT",
                               DataToggle = "tooltip",
                               Icon = "angle-double-left"
                           };

            link.RenderControl(output);
        }

        if (this.CurrentPageIndex > start)
        {
            var link = new ThemeButton
                           {
                               NavigateUrl = this.GetLinkUrl(this.CurrentPageIndex, postBack),
                               Type = ButtonStyle.Secondary,
                               TitleLocalizedPage = "COMMON",
                               TitleLocalizedTag = "GOTOPREVPAGE_TT",
                               DataToggle = "tooltip",
                               Icon = "angle-left"
                           };

            link.RenderControl(output);
        }

        for (var i = start; i < end; i++)
        {
            var page = (i + 1).ToString();

            var link = new ThemeButton
                           {
                               NavigateUrl = this.GetLinkUrl(i + 1, postBack),
                               Type = ButtonStyle.Secondary,
                               TitleLocalizedPage = "COMMON",
                               TitleLocalizedTag = "GOTOPAGE_HEADER",
                               Text = page,
                               DataToggle = "tooltip"
                           };

            if (i == this.CurrentPageIndex)
            {
                link.CssClass = "active";
            }

            link.RenderControl(output);
        }

        if (this.CurrentPageIndex < this.PageCount() - 1)
        {
            var link = new ThemeButton
                           {
                               NavigateUrl = this.GetLinkUrl(this.CurrentPageIndex + 2, postBack),
                               Type = ButtonStyle.Secondary,
                               TitleLocalizedPage = "COMMON",
                               TitleLocalizedTag = "GOTONEXTPAGE_TT",
                               DataToggle = "tooltip",
                               Icon = "angle-right"
                           };

            link.RenderControl(output);
        }

        if (end >= this.PageCount())
        {
            return;
        }

        new ThemeButton
            {
                NavigateUrl = this.GetLinkUrl(this.PageCount(), postBack),
                Type = ButtonStyle.Secondary,
                TitleLocalizedPage = "COMMON",
                TitleLocalizedTag = "GOTOLASTPAGE_TT",
                DataToggle = "tooltip",
                Icon = "angle-double-right"
            }.RenderControl(output);
    }
}