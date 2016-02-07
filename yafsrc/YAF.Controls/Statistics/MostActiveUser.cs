/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2016 Ingo Herbote
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

namespace YAF.Controls.Statistics
{
    #region Using

    using System;
    using System.Data;
    using System.Text;
    using System.Web.UI;

    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    #endregion

    /// <summary>
    /// The most active users.
    /// </summary>
    [ToolboxData("<{0}:MostActiveUsers runat=\"server\"></{0}:MostActiveUsers>")]
    public class MostActiveUsers : BaseControl
    {
        #region Constants and Fields

        /// <summary>
        ///   The _display number.
        /// </summary>
        private int _displayNumber = 10;

        /// <summary>
        ///   The _last num of days.
        /// </summary>
        private int _lastNumOfDays = 7;

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets DisplayNumber.
        /// </summary>
        public int DisplayNumber
        {
            get
            {
                return this._displayNumber;
            }

            set
            {
                this._displayNumber = value;
            }
        }

        /// <summary>
        ///   Gets or sets LastNumOfDays.
        /// </summary>
        public int LastNumOfDays
        {
            get
            {
                return this._lastNumOfDays;
            }

            set
            {
                this._lastNumOfDays = value;
            }
        }

        #endregion

        #region Methods

        /// <summary>
        /// Renders the MostActiveUsers class.
        /// </summary>
        /// <param name="writer">
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            string actRank = string.Empty;

            DataTable rankDt = this.Get<IDataCache>().GetOrSet(
              Constants.Cache.MostActiveUsers,
              () =>
              LegacyDb.user_activity_rank(
                this.PageContext.PageBoardID, DateTime.UtcNow.AddDays(-this.LastNumOfDays), this.DisplayNumber),
              TimeSpan.FromMinutes(5));

            writer.BeginRender();

            var html = new StringBuilder();

            html.AppendFormat(@"<div id=""{0}"" class=""yaf_activeuser"">", this.ClientID);
            html.AppendFormat(@"<h2 class=""yaf_header"">{0}</h2>", "Most Active Users");
            html.AppendFormat(@"<h4 class=""yaf_subheader"">Last {0} Days</h4>", this.LastNumOfDays);

            html.AppendLine("<ol>");

            // flush...
            writer.Write(html.ToString());

            foreach (DataRow row in rankDt.Rows)
            {
                writer.WriteLine("<li>");

                // render UserLink...
                var userLink = new UserLink { UserID = row.Field<int>("ID"), };
                userLink.RenderControl(writer);

                // render online image...
                var onlineStatusImage = new OnlineStatusImage { UserID = row.Field<int>("ID") };
                onlineStatusImage.RenderControl(writer);

                writer.WriteLine(" ");
                writer.WriteLine(@"<span class=""NumberOfPosts"">({0})</span>".FormatWith(row.Field<int>("NumOfPosts")));
                writer.WriteLine("</li>");
            }

            writer.WriteLine("</ol>");
            writer.WriteLine("</div>");
            writer.EndRender();
        }

        #endregion
    }
}
