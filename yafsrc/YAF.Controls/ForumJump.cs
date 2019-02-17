/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
* Copyright (C) 2014-2019 Ingo Herbote
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
    using System.Collections.Specialized;
    using System.Data;
    using System.Web.UI;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Utils;

    #endregion

    /// <summary>
    ///     Summary description for ForumJump.
    /// </summary>
    public class ForumJump : BaseControl, IPostBackDataHandler
    {
        #region Properties

        /// <summary>
        ///     Gets or sets ForumID.
        /// </summary>
        private int ForumID
        {
            get
            {
                return (int)this.ViewState["ForumID"];
            }

            set
            {
                this.ViewState["ForumID"] = value;
            }
        }

        #endregion

        #region Public Methods and Operators

        /// <summary>
        ///     The load post data.
        /// </summary>
        /// <param name="postDataKey">
        ///     The post data key.
        /// </param>
        /// <param name="postCollection">
        ///     The post collection.
        /// </param>
        /// <returns>
        ///     The load post data.
        /// </returns>
        public virtual bool LoadPostData([NotNull] string postDataKey, [NotNull] NameValueCollection postCollection)
        {
            int forumID;
            if (int.TryParse(postCollection[postDataKey], out forumID) && forumID != this.ForumID)
            {
                this.ForumID = forumID;
                return true;
            }

            return false;
        }

        /// <summary>
        ///     The raise post data changed event.
        /// </summary>
        public virtual void RaisePostDataChangedEvent()
        {
            if (this.ForumID == 0)
            {
                YafBuildLink.Redirect(ForumPages.forum);
                return;
            }

            if (this.ForumID < 0)
            {
                // categories are negative
                YafBuildLink.Redirect(ForumPages.forum, "c={0}", -this.ForumID);
                return;
            }

            YafBuildLink.Redirect(ForumPages.topics, "f={0}", this.ForumID);
        }

        #endregion

        #region Methods

        /// <summary>
        ///     The on init.
        /// </summary>
        /// <param name="e">
        ///     The e.
        /// </param>
        protected override void OnInit([NotNull] EventArgs e)
        {
            this.Load += this.Page_Load;
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
            var forumJump =
                this.Get<IDataCache>().GetOrSet(
                    Constants.Cache.ForumJump.FormatWith(
                        this.PageContext.User != null ? this.PageContext.PageUserID.ToString() : "Guest"),
                    () => LegacyDb.forum_listall_sorted(this.PageContext.PageBoardID, this.PageContext.PageUserID),
                    TimeSpan.FromMinutes(5));

            writer.WriteLine(
                @"<select name=""{0}"" onchange=""{1}"" id=""{2}"">".FormatWith(
                    this.UniqueID,
                    this.Page.ClientScript.GetPostBackClientHyperlink(this, this.ID),
                    this.ClientID));

            writer.WriteLine(@"<option value=""0"">{0}</option>", this.Get<YafBoardSettings>().Name);

            int forumID = this.PageContext.PageForumID;
            if (forumID <= 0)
            {
                writer.WriteLine("<option/>");
            }

            foreach (DataRow row in forumJump.Rows)
            {
                writer.WriteLine(
                    @"<option {2}value=""{0}"">&nbsp;&nbsp;{1}</option>".FormatWith(
                        row["ForumID"],
                        this.HtmlEncode(row["Title"]),
                        Convert.ToString(row["ForumID"]) == forumID.ToString() ? @"selected=""selected"" " : string.Empty));
            }

            writer.WriteLine("</select>");
        }

        /// <summary>
        ///     The page_ load.
        /// </summary>
        /// <param name="sender">
        ///     The sender.
        /// </param>
        /// <param name="e">
        ///     The e.
        /// </param>
        private void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            if (!this.Page.IsPostBack)
            {
                this.ForumID = this.PageContext.PageForumID;
            }
        }

        #endregion
    }
}