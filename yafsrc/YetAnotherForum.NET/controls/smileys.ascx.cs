/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014 Ingo Herbote
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
    using System.Data;
    using System.Text;

    using YAF.Classes;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;
    using YAF.Utilities;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Summary description for smileys.
    /// </summary>
    public partial class smileys : BaseUserControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The _dt smileys.
        /// </summary>
        protected DataTable _dtSmileys;

        /// <summary>
        ///   The _onclick.
        /// </summary>
        private string _onclick;

        /// <summary>
        ///   The _perrow.
        /// </summary>
        private int _perrow = 6;

        #endregion

        #region Properties

        /// <summary>
        ///   Sets OnClick.
        /// </summary>
        public string OnClick
        {
            set
            {
                this._onclick = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// The On PreRender event.
        /// </summary>
        /// <param name="e">
        /// the Event Arguments
        /// </param>
        protected override void OnPreRender([NotNull] EventArgs e)
        {
            this.LoadingImage.ImageUrl = YafForumInfo.GetURLToContent("images/loader.gif");
            this.LoadingImage.AlternateText = this.Get<ILocalization>().GetText("COMMON", "LOADING");
            this.LoadingImage.ToolTip = this.Get<ILocalization>().GetText("COMMON", "LOADING");

            this.LoadingText.Text = this.Get<ILocalization>().GetText("COMMON", "LOADING");

            // Setup Pagination js
            YafContext.Current.PageElements.RegisterJsBlock("paginationloadjs", JavaScriptBlocks.PaginationLoadJs);

            base.OnPreRender(e);
        }

        /// <summary>
        /// The page_ load.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The e.
        /// </param>
        protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
        {
            this._dtSmileys = this.GetRepository<Smiley>().ListUnique(this.PageContext.PageBoardID);

            if (this._dtSmileys.Rows.Count == 0)
            {
                this.SmiliesPlaceholder.Visible = false;
            }
            else
            {
                this.CreateSmileys();
            }
        }

        /// <summary>
        /// The create smileys.
        /// </summary>
        private void CreateSmileys()
        {
            var html = new StringBuilder();

            html.Append("<div class=\"result\">");
            html.AppendFormat("<ul class=\"SmilieList\">");

            int rowPanel = 0;

            for (int i = 0; i < this._dtSmileys.Rows.Count; i++)
            {
                DataRow row = this._dtSmileys.Rows[i];
                if (i % this._perrow == 0 && i > 0 && i < this._dtSmileys.Rows.Count)
                {
                    rowPanel++;

                    if (rowPanel == 3)
                    {
                        html.Append("</ul></div>");
                        html.Append("<div class=\"result\">");
                        html.Append("<ul class=\"SmilieList\">\n");

                        rowPanel = 0;
                    }
                }

                string evt = string.Empty;
                if (this._onclick.Length > 0)
                {
                    string strCode = Convert.ToString(row["Code"]).ToLower();
                    strCode = strCode.Replace("&", "&amp;");
                    strCode = strCode.Replace(">", "&gt;");
                    strCode = strCode.Replace("<", "&lt;");
                    strCode = strCode.Replace("\"", "&quot;");
                    strCode = strCode.Replace("\\", "\\\\");
                    strCode = strCode.Replace("'", "\\'");
                    evt = "javascript:{0}('{1} ','{3}{4}/{2}')".FormatWith(
                        this._onclick,
                        strCode,
                        row["Icon"],
                        YafForumInfo.ForumClientFileRoot,
                        YafBoardFolders.Current.Emoticons);
                }
                else
                {
                    evt = "javascript:void()";
                }

                html.AppendFormat(
                    "<li><a tabindex=\"999\" href=\"{2}\"><img src=\"{0}\" alt=\"{1}\" title=\"{1}\" /></a></li>\n",
                    YafBuildLink.Smiley((string)row["Icon"]),
                    row["Emoticon"],
                    evt);
            }

            html.Append("</ul>");
            html.Append("</div>");

            this.SmileyResults.Text = html.ToString();
        }

        #endregion
    }
}