/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
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
namespace YAF.Web.Controls
{
    #region Using

    using System;
    using System.Web;
    using System.Web.UI;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Web.EventsArgs;

    #endregion

    /// <summary>
    /// Pager Control.
    /// </summary>
    public class Pager : BaseControl, IPostBackEventHandler, IPager
    {
        #region Constants and Fields

        /// <summary>
        ///   The _goto page form.
        /// </summary>
        private readonly GotoPageForm gotoPageForm = new GotoPageForm();

        /// <summary>
        ///   The _ignore page index.
        /// </summary>
        private bool ignorePageIndex;

        /// <summary>
        ///   The page change.
        /// </summary>
        public event EventHandler PageChange;

        #endregion

        #region Properties

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

        #endregion

        #region Implemented Interfaces

        #region IPostBackEventHandler

        /// <summary>
        /// The raise post back event.
        /// </summary>
        /// <param name="eventArgument">
        /// The event argument.
        /// </param>
        public void RaisePostBackEvent([NotNull] string eventArgument)
        {
            if (this.PageChange == null)
            {
                return;
            }

            this.CurrentPageIndex = int.Parse(eventArgument) - 1;
            this.ignorePageIndex = true;
            this.PageChange(this, new EventArgs());
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Gets the page URL.
        /// </summary>
        /// <param name="page">The page.</param>
        /// <returns>
        /// The get page url.
        /// </returns>
        protected string GetPageUrl(int page)
        {
            var url = this.PageContext.ForumPageType switch
            {
                ForumPages.Topics => page > 1
                    ? BuildLink.GetLink(
                        ForumPages.Topics,
                        "f={0}&p={1}&name={2}",
                        this.PageContext.PageForumID,
                        page,
                        this.PageContext.PageForumName)
                    : BuildLink.GetLink(
                        ForumPages.Topics,
                        "f={0}&name={1}",
                        this.PageContext.PageForumID,
                        this.PageContext.PageForumName),
                ForumPages.Posts => BuildLink.GetLink(
                    ForumPages.Posts,
                    "t={0}&p={1}&name={2}",
                    this.PageContext.PageTopicID,
                    page,
                    this.PageContext.PageTopicName),
                _ => string.Empty
            };

            return url;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            base.OnInit(e);

            if (!this.ignorePageIndex && this.Get<HttpRequestBase>().QueryString.Exists("p"))
            {
                // set a new page...
                this.CurrentPageIndex =
                    Security.StringToIntOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p")) - 1;
            }

            this.gotoPageForm.ID = this.GetExtendedID("GotoPageForm");

            this.Controls.Add(this.gotoPageForm);

            // hook up events...
            this.gotoPageForm.GotoPageClick += this.GotoPageClick;
        }

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="output">
        /// The output.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter output)
        {
            if (this.PageCount() < 2)
            {
                return;
            }

            output.Write(@"<div class=""btn-toolbar pagination"" role=""toolbar"">");

            output.WriteLine(
                @"<div class=""btn-group mr-2 mb-1"" role=""group"">
                      <button type=""button"" title=""{0}"" class=""btn btn-secondary dropdown-toggle"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                          <i class=""fas fa-copy""></i>&nbsp;{1:N0} {2}
                      </button>",
                this.Get<ILocalization>().TransPage.IsSet()
                    ? this.GetText("COMMON", "GOTOPAGE_HEADER")
                    : "Go to page...",
                this.PageCount(),
                this.GetText("COMMON", "PAGES"));

            output.Write(@"<div class=""dropdown-menu"">");

            output.Write(@"<div class=""px-3 py-1"">");
            output.Write(@"<div class=""mb-3"" data-max=""{0}"" data-min=""1"">", this.PageCount());

            this.gotoPageForm.RenderControl(output);

            output.Write("</div></div>");

            output.Write("</div>");
            output.Write("</div>");

            output.Write(@"<div class=""btn-group mb-1"" role=""group"">");

            this.OutputLinks(output, this.UsePostBack);

            output.WriteLine("</div></div>");
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
        private void OutputLinks([NotNull] HtmlTextWriter output, bool postBack)
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

        /// <summary>
        /// Handles the GotoPageClick event of the _gotoPageForm control.
        /// </summary>
        /// <param name="sender">The source of the event.</param>
        /// <param name="e">The <see cref="GotoPageForumEventArgs"/> instance containing the event data.</param>
        private void GotoPageClick([NotNull] object sender, [NotNull] GotoPageForumEventArgs e)
        {
            var newPage = e.GotoPage - 1;

            if (newPage >= 0 && newPage < this.PageCount())
            {
                // set a new page index...
                this.CurrentPageIndex = newPage;
                this.ignorePageIndex = true;
            }

            this.PageChange?.Invoke(this, new EventArgs());
        }

        #endregion
    }
}