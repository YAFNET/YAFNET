/* Yet Another Forum.NET
 * Copyright (C) 2006-2012 Jaben Cargman
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
 */
namespace YAF.Controls.Statistics
{
  #region Using

    using System;
    using System.Data;
    using System.Text;
    using System.Web.UI;
    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Constants;
    using YAF.Types.Interfaces;
    using YAF.Utils;

  #endregion

    /// <summary>
  /// Control to display last active topics.
  /// </summary>
  [ToolboxData("<{0}:ActiveDiscussions runat=\"server\"></{0}:ActiveDiscussions>")]
  public class ActiveDiscussions : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    ///   Number of active discussions to display in a control.
    /// </summary>
    private int _displayNumber = 10;

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets number of active discussions to display in a control.
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

    #endregion

    #region Methods

    /// <summary>
    /// Renders the ActiveDiscussions control.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
      // for HTML code string representing the control
      var html = new StringBuilder();

      // try to get data from the cache first
      var topicLatest = this.Get<IDataCache>().GetOrSet(
        Constants.Cache.ActiveDiscussions,
        () =>
          {
            // nothing was cached, retrieve it from the database
            DataTable dt = this.Get<IDbFunction>().GetData.topic_latest(
              this.PageContext.PageBoardID,
              this._displayNumber,
              this.PageContext.PageUserID,
              this.Get<YafBoardSettings>().UseStyledNicks,
              this.Get<YafBoardSettings>().NoCountForumsInActiveDiscussions,
              this.Get<YafBoardSettings>().UseReadTrackingByDatabase);

            // Set colorOnly parameter to true, as we get all but color from css in the place
            if (this.Get<YafBoardSettings>().UseStyledNicks)
            {
              var styleTransform = this.Get<IStyleTransform>();
              styleTransform.DecodeStyleByTable(ref dt, false, "LastUserStyle");
            }

            return dt;
          },
        TimeSpan.FromMinutes(this.Get<YafBoardSettings>().ActiveDiscussionsCacheTimeout));

      // render head of control
      html.Append("<table width=\"100%\" class=\"content\" cellspacing=\"1\" border=\"0\" cellpadding=\"0\">");
      html.AppendFormat(
        "<tr><td class=\"header1\">{0}</td></tr>", this.GetText("LATEST_POSTS"));

      // now container for posts themselves
      html.Append("<tr><td class=\"post\">");

      // order of post we are currently rendering
      int currentPost = 1;

      // go through all active topics returned
      foreach (DataRow r in topicLatest.Rows)
      {
        // Output Topic Link
        html.AppendFormat(
          "{2}.&nbsp;<a href=\"{1}\">{0}</a> ({3})", 
          this.Get<IBadWordReplace>().Replace(this.HtmlEncode(Convert.ToString(r["Topic"]))), 
          YafBuildLink.GetLink(ForumPages.posts, "m={0}#{0}", r["LastMessageID"]), 
          currentPost, 
          r["NumPosts"]);

        // new line after topic link
        html.Append("<br />");

        // moving onto next position
        currentPost++;
      }

      // close posts container
      html.Append("</td></tr></table>");

      // render control to the output
      writer.Write(html.ToString());
    }

    #endregion
  }
}