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
    using System.Data;
    using System.Web.UI;
    using YAF.Classes.Data;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;
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
      get
      {
          return this.ViewState["MessageID"] != null ? this.ViewState["MessageID"].ToType<int>() : 0;
      }

        set
      {
        this.ViewState["MessageID"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets Resolved.
    /// </summary>
    [NotNull]
    public virtual string Resolved
    {
      get
      {
        return this.ViewState["Resolved"].ToString();
      }

      set
      {
        this.ViewState["Resolved"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets ResolvedBy. It returns UserID as string value
    /// </summary>
    [NotNull]
    public virtual string ResolvedBy
    {
      get
      {
        return this.ViewState["ResolvedBy"].ToString();
      }

      set
      {
        this.ViewState["ResolvedBy"] = value;
      }
    }

    /// <summary>
    ///   Gets or sets ResolvedDate.
    /// </summary>
    [NotNull]
    public virtual string ResolvedDate
    {
      get
      {
        return this.ViewState["ResolvedDate"].ToString();
      }

      set
      {
        this.ViewState["ResolvedDate"] = value;
      }
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
      // TODO: Needs better commentting.
      writer.WriteLine(@"<div class=""card yafReportedPosts"">");

      var reportersList = LegacyDb.message_listreporters(this.MessageID);

        if (!reportersList.HasRows())
        {
            return;
        }

        writer.BeginRender();

        foreach (DataRow reporter in reportersList.Rows)
        {
            string howMany = null;
            if (reporter["ReportedNumber"].ToType<int>() > 1)
            {
                howMany = "({0})".FormatWith(reporter["ReportedNumber"]);
            }

            // If the message was previously resolved we have not null string
            // and can add an info about last user who resolved the message
            if (this.ResolvedDate.IsSet())
            {
                var resolvedByName = LegacyDb.user_list(
                    this.PageContext.PageBoardID,
                    this.ResolvedBy.ToType<int>(),
                    true).Rows[0]["Name"].ToString();

                var resolvedByDisplayName =
                    UserMembershipHelper.GetDisplayNameFromID(this.ResolvedBy.ToType<long>()).IsSet()
                        ? this.Server.HtmlEncode(
                            this.Get<IUserDisplayName>().GetName(this.ResolvedBy.ToType<int>()))
                        : this.Server.HtmlEncode(resolvedByName);

                writer.Write(
                    @"<div class=""card-header"">{0}<a class=""YafReported_Link"" href=""{1}""> {2}</a> : {3}</div>",
                    this.GetText("RESOLVEDBY"),
                    YafBuildLink.GetLink(
                        ForumPages.profile,
                        "u={0}&name={1}",
                        this.ResolvedBy.ToType<int>(),
                        resolvedByDisplayName),
                    resolvedByDisplayName,
                    this.Get<IDateTime>().FormatDateTimeTopic(this.ResolvedDate));
            }

            writer.Write(
                @"<div class=""card-header"">{3}<a class=""YafReported_Link"" href=""{1}""> {0}{2} </a>",
                !string.IsNullOrEmpty(UserMembershipHelper.GetDisplayNameFromID(reporter["UserID"].ToType<long>()))
                    ? this.Server.HtmlEncode(this.Get<IUserDisplayName>().GetName(reporter["UserID"].ToType<int>()))
                    : this.Server.HtmlEncode(reporter["UserName"].ToString()),
                YafBuildLink.GetLink(
                    ForumPages.profile,
                    "u={0}&name={1}",
                    reporter["UserID"].ToType<int>(),
                    reporter["UserName"].ToString()),
                howMany,
                this.GetText("REPORTEDBY"));
            writer.WriteLine(@"</div>");

            writer.Write(@"<div class=""body"">");

            var reportString = reporter["ReportText"].ToString().Trim().Split('|');

            foreach (var t in reportString)
            {
                var textString = t.Split("??".ToCharArray());
                writer.Write(
                    @"<p class=""card-text"">{0}:</p>",
                    this.Get<IDateTime>().FormatDateTimeTopic(textString[0]));

                // Apply style if a post was previously resolved
                var resStyle = "post_res";
                try
                {
                    if (!(string.IsNullOrEmpty(textString[0]) && string.IsNullOrEmpty(this.ResolvedDate)))
                    {
                        if (Convert.ToDateTime(textString[0]) < Convert.ToDateTime(this.ResolvedDate))
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
            }

            writer.Write(
                @"<a class=""btn btn-primary"" href=""{1}"">{2} {0}</a>",
                !string.IsNullOrEmpty(UserMembershipHelper.GetDisplayNameFromID(reporter["UserID"].ToType<long>()))
                    ? this.Server.HtmlEncode(this.Get<IUserDisplayName>().GetName(reporter["UserID"].ToType<int>()))
                    : this.Server.HtmlEncode(reporter["UserName"].ToString()),
                YafBuildLink.GetLink(
                    ForumPages.pmessage,
                    "u={0}&r={1}",
                    reporter["UserID"].ToType<int>(),
                    this.MessageID),
                this.GetText("REPLYTO"));
        }

        // render controls...
        base.Render(writer);

        writer.WriteLine("</div></div>");
        writer.EndRender();
    }

    #endregion
  }
}