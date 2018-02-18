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

        #endregion

        #region Properties

        /// <summary>
        ///   Gets or sets DisplayNumber.
        /// </summary>
        public int DisplayNumber { get; set; } = 10;

        /// <summary>
        ///   Gets or sets LastNumOfDays.
        /// </summary>
        public int LastNumOfDays { get; set; } = 7;

        #endregion

        #region Methods

        /// <summary>
        /// Renders the MostActiveUsers class.
        /// </summary>
        /// <param name="writer">Das <see cref="T:System.Web.UI.HtmlTextWriter" />-Objekt, das den Inhalt des Serversteuerelements empfängt.</param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            var rankDt = this.Get<IDataCache>().GetOrSet(
              Constants.Cache.MostActiveUsers,
              () =>
              LegacyDb.user_activity_rank(
                this.PageContext.PageBoardID, DateTime.UtcNow.AddDays(-this.LastNumOfDays), this.DisplayNumber),
              TimeSpan.FromMinutes(5));

            writer.BeginRender();

            var html = new StringBuilder();

            html.Append(@"<div class=""card"">");
            html.Append(@"<div class=""card-body"">");
            html.AppendFormat(@"<h5 class=""card-title"">{0}</h5>", "Most Active Users");
            html.AppendFormat(@"<h6 class=""card-subtitle mb-2 text-mutedr"">Last {0} Days</h6>", this.LastNumOfDays);

            html.AppendLine("<ol>");

            // flush...
            writer.Write(html.ToString());

            foreach (DataRow row in rankDt.Rows)
            {
                writer.WriteLine("<li>");

                // render UserLink...
                var userLink = new UserLink { UserID = row.Field<int>("ID"), };
                userLink.RenderControl(writer);

                writer.WriteLine(" ");
                writer.WriteLine(@"<span class=""NumberOfPosts"">({0})</span>".FormatWith(row.Field<int>("NumOfPosts")));
                writer.WriteLine("</li>");
            }

            writer.WriteLine("</ol>");
            writer.WriteLine("</div></div>");
            writer.EndRender();
        }

        #endregion
    }
}
