/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */
namespace YAF.Controls
{
    #region Using

    using System;
    using System.Web;
    using System.Web.UI;

    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;
    using YAF.Utils.Helpers;

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
        ///     Gets or sets GotoPageValue.
        /// </summary>
        public int GotoPageValue { get; set; }

        /// <summary>
        ///   Gets or sets Count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.ViewState["Count"].ToType<int?>() ?? 0;
            }

            set
            {
                this.ViewState["Count"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets CurrentPageIndex.
        /// </summary>
        public int CurrentPageIndex
        {
            get
            {
                return (this.ViewState["CurrentPageIndex"] ?? 0).ToType<int>();
            }

            set
            {
                this.ViewState["CurrentPageIndex"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets LinkedPager.
        /// </summary>
        public string LinkedPager
        {
            get
            {
                return (string)this.ViewState["LinkedPager"];
            }

            set
            {
                this.ViewState["LinkedPager"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets PageSize.
        /// </summary>
        public int PageSize
        {
            get
            {
                return (int?)this.ViewState["PageSize"] ?? 20;
            }

            set
            {
                this.ViewState["PageSize"] = value;
            }
        }

        /// <summary>
        ///   Gets or sets a value indicating whether UsePostBack.
        /// </summary>
        public bool UsePostBack { get; set; } = true;

        /// <summary>
        ///   Gets the Current Linked Pager.
        /// </summary>
        [CanBeNull]
        protected Pager CurrentLinkedPager
        {
            get
            {
                if (this.LinkedPager == null)
                {
                    return null;
                }

                var linkedPager = this.Parent.FindControlAs<Pager>(this.LinkedPager);

                if (linkedPager == null)
                {
                    throw new Exception("Failed to link pager to '{0}'.".FormatWith(this.LinkedPager));
                }

                return linkedPager;
            }
        }

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
            if (this.LinkedPager != null)
            {
                // raise post back event on the linked pager...
                this.CurrentLinkedPager.RaisePostBackEvent(eventArgument);
            }
            else if (this.PageChange != null)
            {
                this.CurrentPageIndex = int.Parse(eventArgument) - 1;
                this.ignorePageIndex = true;
                this.PageChange(this, new EventArgs());
            }
        }

        #endregion

        #endregion

        #region Methods

        /// <summary>
        /// Copies the pager settings.
        /// </summary>
        /// <param name="toPager">To pager.</param>
        protected void CopyPagerSettings([NotNull] Pager toPager)
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
            var url = string.Empty;

            switch (this.PageContext.ForumPageType)
            {
                case ForumPages.topics:
                    url = page > 1
                              ? YafBuildLink.GetLinkNotEscaped(
                                  ForumPages.topics,
                                  "f={0}&p={1}",
                                  this.PageContext.PageForumID,
                                  page)
                              : YafBuildLink.GetLinkNotEscaped(ForumPages.topics, "f={0}", this.PageContext.PageForumID);

                    break;
                case ForumPages.posts:
                    url = page > 1
                              ? YafBuildLink.GetLinkNotEscaped(
                                  ForumPages.posts,
                                  "t={0}&p={1}",
                                  this.PageContext.PageTopicID,
                                  page)
                              : YafBuildLink.GetLinkNotEscaped(ForumPages.posts, "t={0}", this.PageContext.PageTopicID);

                    break;
            }

            return url;
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init" /> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs" /> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            base.OnInit(e);

            if (!this.ignorePageIndex && this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p") != null)
            {
                // set a new page...
                this.CurrentPageIndex =
                    Security.StringToLongOrRedirect(this.Get<HttpRequestBase>().QueryString.GetFirstOrDefault("p"))
                        .ToType<int>() - 1;
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
            if (this.LinkedPager != null)
            {
                // just copy the linked pager settings but still render in this function...
                this.CurrentLinkedPager.CopyPagerSettings(this);
            }

            if (this.PageCount() < 2)
            {
                return;
            }

            output.Write(@"<div class=""btn-toolbar pagination"" role=""toolbar"">");

            output.WriteLine(
                @"<div class=""btn-group"" role=""group"">
                      <button type=""button"" title=""{0}"" class=""btn btn-secondary dropdown-toggle"" data-toggle=""dropdown"" aria-haspopup=""true"" aria-expanded=""false"">
                      {1:N0} {2}</button>",
                this.Get<ILocalization>().TransPage.IsSet()
                    ? this.GetText("COMMON", "GOTOPAGE_HEADER")
                    : "Go to page...",
                this.PageCount(),
                this.GetText("COMMON", "PAGES"));

            output.Write(@"<ul class=""dropdown-menu dropdown-menu-right"">");

            output.Write(@"<li class=""dropdown-item"">");

            this.gotoPageForm.RenderControl(output);

            output.Write("</li>");

            output.Write("</ul></div>");

            output.Write(@"<div class=""btn-group"" role=""group"">");

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
                output.RenderAnchorBegin(
                    this.GetLinkUrl(1, postBack), "btn btn-secondary", this.GetText("COMMON", "GOTOFIRSTPAGE_TT"));

                output.WriteBeginTag("span");
                output.Write(HtmlTextWriter.TagRightChar);

                output.Write("&laquo;");
                output.WriteEndTag("span");
                output.WriteEndTag("a");
            }

            if (this.CurrentPageIndex > start)
            {
                output.RenderAnchorBegin(
                    this.GetLinkUrl(this.CurrentPageIndex, postBack),
                    "btn btn-secondary",
                    this.GetText("COMMON", "GOTOPREVPAGE_TT"));

                output.WriteBeginTag("span");
                output.Write(HtmlTextWriter.TagRightChar);

                output.Write("&lt;");
                output.WriteEndTag("span");
                output.WriteEndTag("a");
            }

            for (var i = start; i < end; i++)
            {
                if (i == this.CurrentPageIndex)
                {
                    output.WriteBeginTag("span");
                    output.WriteAttribute("class", "btn btn-primary");
                    output.Write(HtmlTextWriter.TagRightChar);
                    output.Write(i + 1);
                    output.WriteEndTag("span");
                }
                else
                {
                    var page = (i + 1).ToString();

                    output.RenderAnchorBegin(this.GetLinkUrl(i + 1, postBack), "btn btn-secondary", page);

                    output.WriteBeginTag("span");
                    output.Write(HtmlTextWriter.TagRightChar);

                    output.Write(page);
                    output.WriteEndTag("span");
                    output.WriteEndTag("a");
                }
            }

            if (this.CurrentPageIndex < (this.PageCount() - 1))
            {
                output.RenderAnchorBegin(
                    this.GetLinkUrl(this.CurrentPageIndex + 2, postBack),
                    "btn btn-secondary",
                    this.GetText("COMMON", "GOTONEXTPAGE_TT"));

                output.WriteBeginTag("span");
                output.Write(HtmlTextWriter.TagRightChar);

                output.Write("&gt;");
                output.WriteEndTag("span");
                output.WriteEndTag("a");
            }

            if (end >= this.PageCount())
            {
                return;
            }

            output.RenderAnchorBegin(
                this.GetLinkUrl(this.PageCount(), postBack), "btn btn-secondary", this.GetText("COMMON", "GOTOLASTPAGE_TT"));

            output.WriteBeginTag("span");
            output.Write(HtmlTextWriter.TagRightChar);

            output.Write("&raquo;");
            output.WriteEndTag("span");
            output.WriteEndTag("a");
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

            if (this.LinkedPager != null)
            {
                // raise post back event on the linked pager...
                this.CurrentLinkedPager.GotoPageClick(sender, e);
            }
            else
            {
                this.PageChange?.Invoke(this, new EventArgs());
            }
        }

        #endregion
    }
}