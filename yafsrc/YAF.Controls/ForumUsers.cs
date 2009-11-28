/* Yet Another Forum.NET
 * Copyright (C) 2006-2009 Jaben Cargman
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
using System;
using System.Data;
using System.Web.UI;
using YAF.Classes.Core;
using YAF.Classes.Data;
using YAF.Classes.UI;

namespace YAF.Controls
{
  /// <summary>
  /// Summary description for ForumUsers.
  /// </summary>
  public class ForumUsers : BaseControl
  {
    /// <summary>
    /// The _active users.
    /// </summary>
    private ActiveUsers _activeUsers = new ActiveUsers();

    /// <summary>
    /// Initializes a new instance of the <see cref="ForumUsers"/> class.
    /// </summary>
    public ForumUsers()
    {
      this._activeUsers.ID = GetUniqueID("ActiveUsers");
      Load += new EventHandler(ForumUsers_Load);
    }

    /// <summary>
    /// Gets or sets a value indicating whether TreatGuestAsHidden.
    /// </summary>
    public bool TreatGuestAsHidden
    {
      get
      {
        return this._activeUsers.TreatGuestAsHidden;
      }

      set
      {
        this._activeUsers.TreatGuestAsHidden = value;
      }
    }

    /// <summary>
    /// The forum users_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumUsers_Load(object sender, EventArgs e)
    {
      bool bTopic = PageContext.PageTopicID > 0;

      if (this._activeUsers.ActiveUserTable == null)
      {
        this._activeUsers.ActiveUserTable =
          YafServices.DBBroker.StyleTransformDataTable(
            bTopic
              ? DB.active_listtopic(PageContext.PageTopicID, PageContext.BoardSettings.UseStyledNicks)
              : DB.active_listforum(PageContext.PageForumID, PageContext.BoardSettings.UseStyledNicks));
      }

      // add it...
      Controls.Add(this._activeUsers);
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render(HtmlTextWriter writer)
    {
      // Ederon : 07/14/2007
      if (!PageContext.BoardSettings.ShowBrowsingUsers)
      {
        return;
      }

      bool bTopic = PageContext.PageTopicID > 0;

      if (bTopic)
      {
        writer.WriteLine(String.Format(@"<tr id=""{0}"" class=""header2"">", ClientID));
        writer.WriteLine(String.Format("<td colspan=\"3\">{0}</td>", PageContext.Localization.GetText("TOPICBROWSERS")));
        writer.WriteLine("</tr>");
        writer.WriteLine("<tr class=\"post\">");
        writer.WriteLine("<td colspan=\"3\">");
      }
      else
      {
        writer.WriteLine(String.Format(@"<tr id=""{0}"" class=""header2"">", ClientID));
        writer.WriteLine(String.Format("<td colspan=\"6\">{0}</td>", PageContext.Localization.GetText("FORUMUSERS")));
        writer.WriteLine("</tr>");
        writer.WriteLine("<tr class=\"post\">");
        writer.WriteLine("<td colspan=\"6\">");
      }

      base.Render(writer);

      writer.WriteLine("</td>");
      writer.WriteLine("</tr>");
    }
  }
}