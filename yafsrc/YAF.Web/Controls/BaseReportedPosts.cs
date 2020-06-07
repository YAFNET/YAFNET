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

    using YAF.Configuration;
    using YAF.Core.BaseControls;
    using YAF.Core.Extensions;
    using YAF.Core.Model;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
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

                        var resolvedByName = string.Empty;

                        var reporterUser = this.GetRepository<User>().GetById(reporter["UserID"].ToType<int>());

                        var reporterName = this.Get<BoardSettings>().EnableDisplayName
                            ? reporterUser.DisplayName
                            : reporterUser.Name;

                        // If the message was previously resolved we have not null string
                        // and can add an info about last user who resolved the message
                        if (this.ResolvedDate.IsSet()  && DateTime.Parse(this.ResolvedDate) > DateTime.MinValue)
                        {
                            var resolvedBy = this.GetRepository<User>().GetById(
                                this.ResolvedBy.ToType<int>());

                            resolvedByName = this.Get<BoardSettings>().EnableDisplayName
                                ? resolvedBy.DisplayName
                                : resolvedBy.Name;

                            writer.Write(
                                @"<span class=""font-weight-bold"">{0}</span><a href=""{1}""> {2}</a> : {3}",
                                this.GetText("RESOLVEDBY"),
                                BuildLink.GetUserProfileLink(this.ResolvedBy.ToType<int>(), resolvedByName),
                                resolvedByName,
                                this.Get<IDateTime>().FormatDateTimeTopic(this.ResolvedDate));
                        }

                        writer.Write(
                            @"<span class=""font-weight-bold"">{3}</span><a href=""{1}""> {0}{2} </a>",
                            resolvedByName,
                            BuildLink.GetUserProfileLink(reporter["UserID"].ToType<int>(), reporterName),
                            howMany,
                            this.GetText("REPORTEDBY"));

                        writer.Write(
                            @"<a class=""btn btn-secondary btn-sm"" href=""{1}""><i class=""fa fa-envelope fa-fw""></i>&nbsp;{2} {0}</a>",
                            reporterName,
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