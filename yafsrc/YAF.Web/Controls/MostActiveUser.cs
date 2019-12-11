/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2019 Ingo Herbote
 * https://www.yetanotherforum.net/
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

namespace YAF.Web.Controls
{
    #region Using

    using System;
    using System.Data;
    using System.Web.UI;

    using YAF.Core.BaseControls;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Models;

    #endregion

    /// <summary>
    /// The most active users.
    /// </summary>
    [ToolboxData("<{0}:MostActiveUsers runat=\"server\"></{0}:MostActiveUsers>")]
    public class MostActiveUsers : BaseControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets DisplayNumber.
        /// </summary>
        public int DisplayNumber { get; set; } = 10;

        /// <summary>
        ///   Gets or sets Number of Days.
        /// </summary>
        public int LastNumOfDays { get; set; } = 7;

        #endregion

        #region Methods

        /// <summary>
        /// Renders the Most Active Users Card.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            var rankDt = this.Get<IDataCache>().GetOrSet(
                Constants.Cache.MostActiveUsers,
                () => this.GetRepository<User>().ActivityRankAsDataTable(
                    this.PageContext.PageBoardID,
                    DateTime.UtcNow.AddDays(-this.LastNumOfDays),
                    this.DisplayNumber),
                TimeSpan.FromMinutes(5));

            if (!rankDt.HasRows())
            {
                return;
            }

            writer.BeginRender();

            writer.Write(@"<div class=""card mb-3"">");
            writer.Write(@"<div class=""card-header""><span class=""fa-stack"">");
            writer.Write(
                @"<i class=""fas fa-chart-line fa-2x fa-fw text-secondary""></i></span>&nbsp;{0}</div>",
                this.GetTextFormatted("MOST_ACTIVE", this.LastNumOfDays));
            writer.Write(@"<div class=""card-body"">");

            writer.Write("<ol>");

            rankDt.AsEnumerable().ForEach(
                row =>
                    {
                        writer.Write("<li>");

                        // render UserLink...
                        var userLink = new UserLink { UserID = row.Field<int>("ID"), };
                        userLink.RenderControl(writer);

                        writer.Write(" ");
                        writer.Write($@"<span class=""NumberOfPosts"">({row.Field<int>("NumOfPosts")})</span>");
                        writer.Write("</li>");
                    });

            writer.Write("</ol>");
            writer.Write("</div></div>");
            writer.EndRender();
        }

        #endregion
    }
}