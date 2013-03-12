/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
 * http://www.yetanotherforum.net/
 * 
 * This program is free software; you can redistribute it and/or
 * modify it under the terms of the GNU General Public License
 * as published by the Free Software Foundation; either version 2
 * of the License, or (at your option) any later version.
 * 
 * This program is distributed in the hope that it will be useful,
 * but WITHOUT ANY WARRANTY; without even the implied warranty of
 * MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
 * GNU General Public License for more details.
 * 
 * You should have received a copy of the GNU General Public License
 * along with this program; if not, write to the Free Software
 * Foundation, Inc., 59 Temple Place - Suite 330, Boston, MA  02111-1307, USA.
 * Written by vzrus (c) 2009 for Yet Another Forum.NET  */
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
      writer.WriteLine(@"<div id=""{0}"" class=""yafReportedPosts"">".FormatWith(this.ClientID));

      DataTable reportersList = LegacyDb.message_listreporters(this.MessageID);

        if (reportersList.Rows.Count <= 0)
        {
            return;
        }

        int i = 0;
        writer.BeginRender();

        foreach (DataRow reporter in reportersList.Rows)
        {
            string howMany = null;
            if (reporter["ReportedNumber"].ToType<int>() > 1)
            {
                howMany = "({0})".FormatWith(reporter["ReportedNumber"]);
            }

            writer.WriteLine(
                @"<table cellspacing=""0"" cellpadding=""0"" class=""content"" id=""yafreportedtable{0}"" style=""width:100%"">", this.ClientID);

            // If the message was previously resolved we have not null string
            // and can add an info about last user who resolved the message
            if (!string.IsNullOrEmpty(this.ResolvedDate))
            {
                string resolvedByName =
                    LegacyDb.user_list(this.PageContext.PageBoardID, this.ResolvedBy.ToType<int>(), true).Rows[0]["Name"].ToString();

                writer.Write(@"<tr class=""header2""><td>");
                writer.Write(
                    @"<span class=""postheader"">{0}</span><a class=""YafReported_Link"" href=""{1}""> {2}</a><span class=""YafReported_ResolvedBy""> : {3}</span>", 
                    this.GetText("RESOLVEDBY"), 
                    YafBuildLink.GetLink(ForumPages.profile, "u={0}", this.ResolvedBy.ToType<int>()), 
                    !string.IsNullOrEmpty(UserMembershipHelper.GetDisplayNameFromID(this.ResolvedBy.ToType<long>()))
                        ? this.Server.HtmlEncode(this.Get<IUserDisplayName>().GetName(this.ResolvedBy.ToType<int>()))
                        : this.Server.HtmlEncode(resolvedByName), 
                    this.Get<IDateTime>().FormatDateTimeTopic(this.ResolvedDate));
                writer.WriteLine(@"</td></tr>");
            }

            writer.Write(@"<tr class=""header2""><td>");
            writer.Write(
                @"<span class=""YafReported_Complainer"">{3}</span><a class=""YafReported_Link"" href=""{1}""> {0}{2} </a>", 
                !string.IsNullOrEmpty(UserMembershipHelper.GetDisplayNameFromID(reporter["UserID"].ToType<long>()))
                    ? this.Server.HtmlEncode(this.Get<IUserDisplayName>().GetName(reporter["UserID"].ToType<int>()))
                    : this.Server.HtmlEncode(reporter["UserName"].ToString()), 
                YafBuildLink.GetLink(ForumPages.profile, "u={0}", reporter["UserID"].ToType<int>()), 
                howMany, 
                this.GetText("REPORTEDBY"));
            writer.WriteLine(@"</td></tr>");

            string[] reportString = reporter["ReportText"].ToString().Trim().Split('|');

            foreach (string t in reportString)
            {
                string[] textString = t.Split("??".ToCharArray());
                writer.Write(@"<tr class=""post""><td>");
                writer.Write(
                    @"<span class=""YafReported_DateTime"">{0}:</span>", 
                    this.Get<IDateTime>().FormatDateTimeTopic(textString[0]));

                // Apply style if a post was previously resolved
                string resStyle = "post_res";
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
                    writer.Write(@"<tr><td class=""{0}"">", resStyle);
                    writer.Write(textString[2]);
                    writer.WriteLine(@"</td></tr>");
                }
                else
                {
                    writer.WriteLine(@"<tr  class=""post""><td>");
                    writer.Write(t);
                    writer.WriteLine(@"</td></tr>");
                }
            }

            writer.WriteLine(@"<tr class=""postfooter""><td>");

            writer.Write(
                @"<a class=""YafReported_Link"" href=""{1}"">{2} {0}</a>", 
                !string.IsNullOrEmpty(UserMembershipHelper.GetDisplayNameFromID(reporter["UserID"].ToType<long>()))
                    ? this.Server.HtmlEncode(this.Get<IUserDisplayName>().GetName(reporter["UserID"].ToType<int>()))
                    : this.Server.HtmlEncode(reporter["UserName"].ToString()), 
                YafBuildLink.GetLink(
                    ForumPages.pmessage, "u={0}&r={1}", reporter["UserID"].ToType<int>(), this.MessageID), 
                this.GetText("REPLYTO"));

            writer.WriteLine(@"</td></tr>");

            // TODO: Remove hard-coded formatting.
            if (i < reportersList.Rows.Count - 1)
            {
                writer.Write("</table></br>");
            }
            else
            {
                writer.WriteLine(@"</td></tr>");
            }

            i++;
        }

        // render controls...
        writer.Write(@"</table>");
        base.Render(writer);

        writer.WriteLine("</div>");
        writer.EndRender();
    }

    #endregion
  }
}