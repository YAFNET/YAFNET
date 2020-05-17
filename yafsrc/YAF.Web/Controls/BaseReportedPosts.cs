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
    using System.Data;
    using System.Web.UI;

    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
    using YAF.Types.Interfaces.Identity;
    using YAF.Types.Models;
    using YAF.Utils;

    #endregion

    /// <summary>
    /// Shows a Reporters for reported posts
    /// </summary>
    public class BaseReportedPosts : BaseUserControl
    {
        #region Properties

        /// <summary>
        ///   Gets or sets MessageID.
        /// </summary>
        public virtual int MessageID
        {
            get => this.ViewState["MessageID"]?.ToType<int>() ?? 0;

            set => this.ViewState["MessageID"] = value;
        }

        /// <summary>
        ///   Gets or sets Resolved.
        /// </summary>
        [NotNull]
        public virtual string Resolved
        {
            get => this.ViewState["Resolved"].ToString();

            set => this.ViewState["Resolved"] = value;
        }

        /// <summary>
        ///   Gets or sets ResolvedBy. It returns UserID as string value
        /// </summary>
        [NotNull]
        public virtual int ResolvedBy
        {
            get => this.ViewState["ResolvedBy"].ToType<int>();

            set => this.ViewState["ResolvedBy"] = value;
        }

        /// <summary>
        ///   Gets or sets ResolvedDate.
        /// </summary>
        [NotNull]
        public virtual string ResolvedDate
        {
            get => this.ViewState["ResolvedDate"].ToString();

            set => this.ViewState["ResolvedDate"] = value;
        }

        #endregion

        #region Methods

        /// <summary>
        /// The render.
        /// </summary>
        /// <param name="writer">
        /// The writer.
        /// </param>
        protected override void Render([NotNull] HtmlTextWriter writer)
        {
            var reportersList = this.GetRepository<Message>().ListReportersAsDataTable(this.MessageID);

            if (!reportersList.HasRows())
            {
                return;
            }

            writer.BeginRender();
            reportersList.AsEnumerable().ForEach(
                reporter =>
                    {
                        writer.WriteLine(@"<div class=""alert alert-secondary"" role=""alert"">");

                        string howMany = null;
                        if (reporter["ReportedNumber"].ToType<int>() > 1)
                        {
                            howMany = $"({reporter["ReportedNumber"]})";
                        }

                        // If the message was previously resolved we have not null string
                        // and can add an info about last user who resolved the message
                        if (this.ResolvedDate.IsSet())
                        {
                            var resolvedByName = this.GetRepository<User>().ListAsDataTable(
                                this.PageContext.PageBoardID,
                                this.ResolvedBy.ToType<int>(),
                                true).Rows[0]["Name"].ToString();

                            var resolvedByDisplayName =
                                this.Get<IAspNetUsersHelper>().GetDisplayNameFromID(this.ResolvedBy).IsSet()
                                    ? this.Server.HtmlEncode(
                                        this.Get<IUserDisplayName>().GetName(this.ResolvedBy.ToType<int>()))
                                    : this.Server.HtmlEncode(resolvedByName);

                            writer.Write(
                                @"<span class=""font-weight-bold"">{0}</span><a href=""{1}""> {2}</a> : {3}",
                                this.GetText("RESOLVEDBY"),
                                BuildLink.GetLink(
                                    ForumPages.UserProfile,
                                    "u={0}&name={1}",
                                    this.ResolvedBy.ToType<int>(),
                                    resolvedByDisplayName),
                                resolvedByDisplayName,
                                this.Get<IDateTime>().FormatDateTimeTopic(this.ResolvedDate));
                        }

                        writer.Write(
                            @"<span class=""font-weight-bold"">{3}</span><a href=""{1}""> {0}{2} </a>",
                            this.Get<IAspNetUsersHelper>().GetDisplayNameFromID(reporter["UserID"].ToType<int>()).IsSet()
                                ? this.Server.HtmlEncode(
                                    this.Get<IUserDisplayName>().GetName(reporter["UserID"].ToType<int>()))
                                : this.Server.HtmlEncode(reporter["UserName"].ToString()),
                            BuildLink.GetLink(
                                ForumPages.UserProfile,
                                "u={0}&name={1}",
                                reporter["UserID"].ToType<int>(),
                                reporter["UserName"].ToString()),
                            howMany,
                            this.GetText("REPORTEDBY"));

                        writer.Write(
                            @"<a class=""btn btn-secondary btn-sm"" href=""{1}""><i class=""fa fa-envelope fa-fw""></i>&nbsp;{2} {0}</a>",
                            this.Get<IAspNetUsersHelper>().GetDisplayNameFromID(reporter["UserID"].ToType<int>()).IsSet()
                                ? this.Server.HtmlEncode(
                                    this.Get<IUserDisplayName>().GetName(reporter["UserID"].ToType<int>()))
                                : this.Server.HtmlEncode(reporter["UserName"].ToString()),
                            BuildLink.GetLink(
                                ForumPages.PostPrivateMessage,
                                "u={0}&r={1}",
                                reporter["UserID"].ToType<int>(),
                                this.MessageID),
                            this.GetText("REPLYTO"));

                        var reportString = reporter["ReportText"].ToString().Trim().Split('|');

                        reportString.ForEach(
                            t =>
                                {
                                    var textString = t.Split("??".ToCharArray());

                                    writer.Write(
                                        @"<span class=""font-weight-bold pl-1"">@</span><span class=""pl-1"">{0}</span>",
                                        this.Get<IDateTime>().FormatDateTimeTopic(textString[0]));

                                    // Apply style if a post was previously resolved
                                    var resStyle = "post_res";
                                    try
                                    {
                                        if (!(textString[0].IsNotSet()
                                              && this.ResolvedDate.IsNotSet()))
                                        {
                                            if (Convert.ToDateTime(textString[0])
                                                < Convert.ToDateTime(this.ResolvedDate))
                                            {
                                                resStyle = "post";
                                            }
                                        }
                                    }
                                    catch (Exception)
                                    {
                                        resStyle = "post_res";
                                    }

                                    if (textString.Length > 2)
                                    {
                                        writer.Write(@"<p class=""card-text {0}"">", resStyle);
                                        writer.Write(textString[2]);
                                        writer.WriteLine(@"</p>");
                                    }
                                    else
                                    {
                                        writer.WriteLine(@"<p class=""card-text"">");
                                        writer.Write(t);
                                        writer.WriteLine(@"</p>");
                                    }
                                });

                        writer.Write("</div>");
                    });

            // render controls...
            base.Render(writer);

            writer.EndRender();
        }

        #endregion
    }
}