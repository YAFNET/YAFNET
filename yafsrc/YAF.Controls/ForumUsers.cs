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

namespace YAF.Controls
{
  #region Using

  using System;
  using System.Web.UI;

  using YAF.Core; using YAF.Types.Interfaces; using YAF.Types.Constants;
  using YAF.Classes.Data;
  using YAF.Utils;
  using YAF.Types;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// Summary description for ForumUsers.
  /// </summary>
  public class ForumUsers : BaseControl
  {
    #region Constants and Fields

    /// <summary>
    ///   The _active users.
    /// </summary>
    private readonly ActiveUsers _activeUsers = new ActiveUsers();

    #endregion

    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ForumUsers" /> class.
    /// </summary>
    public ForumUsers()
    {
      this._activeUsers.ID = this.GetUniqueID("ActiveUsers");
      this.Load += this.ForumUsers_Load;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets or sets a value indicating whether TreatGuestAsHidden.
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
      // Ederon : 07/14/2007
      if (!this.PageContext.BoardSettings.ShowBrowsingUsers)
      {
        return;
      }

      bool bTopic = this.PageContext.PageTopicID > 0;

      if (bTopic)
      {
        writer.WriteLine(@"<tr id=""{0}"" class=""header2"">".FormatWith(this.ClientID));
        writer.WriteLine(
          "<td colspan=\"3\">{0}</td>".FormatWith(this.GetText("TOPICBROWSERS")));
        writer.WriteLine("</tr>");
        writer.WriteLine("<tr class=\"post\">");
        writer.WriteLine("<td colspan=\"3\">");
      }
      else
      {
        writer.WriteLine(@"<tr id=""{0}"" class=""header2"">".FormatWith(this.ClientID));
        writer.WriteLine("<td colspan=\"6\">{0}</td>".FormatWith(this.GetText("FORUMUSERS")));
        writer.WriteLine("</tr>");
        writer.WriteLine("<tr class=\"post\">");
        writer.WriteLine("<td colspan=\"6\">");
      }

      base.Render(writer);

      writer.WriteLine("</td>");
      writer.WriteLine("</tr>");
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
    private void ForumUsers_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      bool bTopic = this.PageContext.PageTopicID > 0;

      if (this._activeUsers.ActiveUserTable == null)
      {
        this._activeUsers.ActiveUserTable =
          this.Get<IDBBroker>().StyleTransformDataTable(
            bTopic
              ? LegacyDb.active_listtopic(this.PageContext.PageTopicID, this.PageContext.BoardSettings.UseStyledNicks)
              : LegacyDb.active_listforum(this.PageContext.PageForumID, this.PageContext.BoardSettings.UseStyledNicks));
      }

      // add it...
      this.Controls.Add(this._activeUsers);
    }

    #endregion
  }
}