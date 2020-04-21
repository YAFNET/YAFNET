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
    using System.Collections.Specialized;
    using System.Data;
    using System.Linq;
    using System.Web.UI;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// The Forum Jump Control
    /// </summary>
    /// <seealso cref="BaseControl" />
    /// <seealso cref="System.Web.UI.IPostBackDataHandler" />
    public class ForumJump : BaseControl, IPostBackDataHandler
    {
        #region Properties

        /// <summary>
        ///     Gets or sets ForumID.
        /// </summary>
        private int ForumId
        {
            get => this.ViewState["ForumID"].ToType<int>();

            set => this.ViewState["ForumID"] = value;
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        /// Load Post Data
        /// </summary>
        /// <param name="postDataKey">The post data key.</param>
        /// <param name="postCollection">The post collection.</param>
        /// <returns>
        /// The load post data.
        /// </returns>
        public virtual bool LoadPostData([NotNull] string postDataKey, [NotNull] NameValueCollection postCollection)
        {
            if (!int.TryParse(postCollection[postDataKey], out var forumId) || forumId == this.ForumId)
            {
                return false;
            }

            this.ForumId = forumId;
            return true;
        }

        /// <summary>
        ///     The raise post data changed event.
        /// </summary>
        public virtual void RaisePostDataChangedEvent()
        {
            if (this.ForumId == 0)
            {
                BuildLink.Redirect(ForumPages.Board);
                return;
            }

            if (this.ForumId < 0)
            {
                // categories are negative
                BuildLink.Redirect(ForumPages.Board, "c={0}", -this.ForumId);
                return;
            }

            BuildLink.Redirect(ForumPages.Topics, "f={0}", this.ForumId);
        }

        #endregion

        #region Methods

        /// <summary>Raises the <see cref="E:System.Web.UI.Control.Init"/> event.</summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.Load += this.PageLoad;
            base.OnInit(e);
        }

        /// <summary>
        ///     The render.
        /// </summary>
        /// <param name="writer">
        ///     The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            var forumJump = this.Get<IDataCache>().GetOrSet(
                string.Format(
                    Constants.Cache.ForumJump,
                    this.PageContext.User != null ? this.PageContext.PageUserID.ToString() : "Guest"),
                () => this.GetRepository<Types.Models.Forum>().ListAllSortedAsDataTable(
                    this.PageContext.PageBoardID,
                    this.PageContext.PageUserID),
                TimeSpan.FromMinutes(5));

            writer.WriteLine(
                $@"<select name=""{this.UniqueID}"" 
                             onchange=""{this.Page.ClientScript.GetPostBackClientHyperlink(this, this.ID)}"" 
                             id=""{this.ClientID}"" 
                             class=""select2-image-select"">");

            var forumId = this.PageContext.PageForumID;
            if (forumId <= 0)
            {
                writer.WriteLine("<option/>");
            }

            forumJump.Rows.Cast<DataRow>().ForEach(
                row =>
                    {
                        var title = this.HtmlEncode(row["Title"]);

                        writer.WriteLine(
                            @"<option {2}value=""{0}"" data-content=""{3}"">&nbsp;&nbsp;{1}</option>",
                            row["ForumID"],
                            title,
                            row["ForumID"].ToString() == forumId.ToString()
                                ? @"selected=""selected"" "
                                : string.Empty,
                            $"<span class='select2-image-select-icon'><i class='fas fa-{row["Icon"]} fa-fw text-secondary'></i>&nbsp;{title}</span>");
                    });

            writer.WriteLine("</select>");
        }

        /// <summary>
        /// Page Load Event
        /// </summary>
        /// <param name="sender">The sender.</param>
        /// <param name="e">The <see cref="EventArgs"/> instance containing the event data.</param>
        private void PageLoad([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.ForumId = this.PageContext.PageForumID;
            }
        }

        #endregion
    }
}