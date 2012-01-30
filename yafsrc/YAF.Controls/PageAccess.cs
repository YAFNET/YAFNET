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
  using YAF.Types;

  #endregion

  /// <summary>
  /// Summary description for PageLinks.
  /// </summary>
  public class PageAccess : BaseControl
  {
    #region Methods

    /// <summary>
    /// The on init.
    /// </summary>
    /// <param name="e">
    /// The e.
    /// </param>
    protected override void OnInit([NotNull] EventArgs e)
    {
      Load += new EventHandler(this.Page_Load);
      base.OnInit(e);
    }

    /// <summary>
    /// The render.
    /// </summary>
    /// <param name="writer">
    /// The writer.
    /// </param>
    protected override void Render([NotNull] HtmlTextWriter writer)
    {
      writer.WriteLine(this.GetText(PageContext.ForumPostAccess ? "can_post" : "cannot_post"));
      writer.WriteLine("<br />");
      writer.WriteLine(this.GetText(PageContext.ForumReplyAccess ? "can_reply" : "cannot_reply"));
      writer.WriteLine("<br />");
      writer.WriteLine(this.GetText(PageContext.ForumDeleteAccess ? "can_delete" : "cannot_delete"));
      writer.WriteLine("<br />");
      writer.WriteLine(this.GetText(PageContext.ForumEditAccess ? "can_edit" : "cannot_edit"));
      writer.WriteLine("<br />");
      writer.WriteLine(this.GetText(PageContext.ForumPollAccess ? "can_poll" : "cannot_poll"));
      writer.WriteLine("<br />");
      writer.WriteLine(this.GetText(PageContext.ForumVoteAccess ? "can_vote" : "cannot_vote"));
      writer.WriteLine("<br />");
    }

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
    }

    #endregion
  }
}