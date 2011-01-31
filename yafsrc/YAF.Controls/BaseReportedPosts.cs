/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Classes.Data;
  using YAF.Utils;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

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
        if (this.ViewState["MessageID"] != null)
        {
          return Convert.ToInt32(this.ViewState["MessageID"]);
        }

        return 0;
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

      if (reportersList.Rows.Count > 0)
      {
        int i = 0;
        writer.BeginRender();

        foreach (DataRow reporter in reportersList.Rows)
        {
          string howMany = null;
          if (Convert.ToInt32(reporter["ReportedNumber"]) > 1)
          {
            howMany = "(" + reporter["ReportedNumber"] + ")";
          }

          writer.WriteLine(
            @"<table cellspacing=""0"" cellpadding=""0"" class=""content"" id=""yafreportedtable{0}"">", this.ClientID);

          // If the message was previously resolved we have not null string
          // and can add an info about last user who resolved the message
          if (!string.IsNullOrEmpty(this.ResolvedDate))
          {
            string resolvedByName =
              LegacyDb.user_list(this.PageContext.PageBoardID, Convert.ToInt32(this.ResolvedBy), true).Rows[0]["Name"].
                ToString();

            writer.Write(@"<tr><td class=""header2"">");
            writer.Write(
              @"<span class=""postheader"">{0}</span><a class=""YafReported_Link"" href=""{1}""> {2}</a><span class=""YafReported_ResolvedBy""> : {3}</span>", 
              this.GetText("RESOLVEDBY"), 
              YafBuildLink.GetLink(ForumPages.profile, "u={0}", Convert.ToInt32(this.ResolvedBy)), 
              !string.IsNullOrEmpty(UserMembershipHelper.GetDisplayNameFromID(Convert.ToInt64(this.ResolvedBy)))
                ? this.Server.HtmlEncode(this.Get<IUserDisplayName>().GetName(Convert.ToInt32(this.ResolvedBy)))
                : this.Server.HtmlEncode(resolvedByName), 
              this.Get<IDateTime>().FormatDateTimeTopic(this.ResolvedDate));
            writer.WriteLine(@"</td></tr>");
          }

          writer.Write(@"<tr><td class=""post"">");
          writer.Write(@"<tr><td class=""header2"">");
          writer.Write(
            @"<span class=""YafReported_Complainer"">{3}</span><a class=""YafReported_Link"" href=""{1}""> {0}{2} </a>", 
            !string.IsNullOrEmpty(UserMembershipHelper.GetDisplayNameFromID(Convert.ToInt64(reporter["UserID"])))
              ? this.Server.HtmlEncode(this.Get<IUserDisplayName>().GetName(Convert.ToInt32(reporter["UserID"])))
              : this.Server.HtmlEncode(reporter["UserName"].ToString()), 
            YafBuildLink.GetLink(ForumPages.profile, "u={0}", Convert.ToInt32(reporter["UserID"])), 
            howMany, 
            this.GetText("REPORTEDBY"));
          writer.WriteLine(@"</td></tr>");

          string[] reportString = reporter["ReportText"].ToString().Trim().Split('|');

          for (int istr = 0; istr < reportString.Length; istr++)
          {
            string[] textString = reportString[istr].Split("??".ToCharArray());
            writer.Write(@"<tr><td class=""post"">");
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
              writer.WriteLine(@"<tr><td class=""post"">");
              writer.Write(reportString[istr]);
              writer.WriteLine(@"</td></tr>");
            }
          }

          writer.WriteLine(@"<tr><td class=""postfooter"">");
          writer.Write(
            @"<a class=""YafReported_Link"" href=""{1}"">{2} {0}</a>", 
            !string.IsNullOrEmpty(UserMembershipHelper.GetDisplayNameFromID(Convert.ToInt64(reporter["UserID"])))
              ? this.Server.HtmlEncode(this.Get<IUserDisplayName>().GetName(Convert.ToInt32(reporter["UserID"])))
              : this.Server.HtmlEncode(reporter["UserName"].ToString()), 
            YafBuildLink.GetLink(
              ForumPages.pmessage, "u={0}&r={1}", Convert.ToInt32(reporter["UserID"]), this.MessageID), 
            this.GetText("REPLYTO"));
          writer.WriteLine(@"</td></tr>");

          // TODO: Remove hard-coded formatting.
          if (i < reportersList.Rows.Count - 1)
          {
            writer.Write("<br></br>");
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
    }

    #endregion
  }
}