/* Yet Another Forum.NET
 * Copyright (C) 2006-2010 Jaben Cargman
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
  using System;
  using YAF.Classes;
  using YAF.Classes.Core;
  using YAF.Classes.Utils;

  /// <summary>
  /// The forum welcome.
  /// </summary>
  public partial class ForumWelcome : BaseUserControl
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="ForumWelcome"/> class.
    /// </summary>
    public ForumWelcome()
    {
      PreRender += new EventHandler(this.ForumWelcome_PreRender);
    }

    /// <summary>
    /// The forum welcome_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumWelcome_PreRender(object sender, EventArgs e)
    {
        this.TimeNow.Text = this.PageContext.IsGuest ? string.Format("{0}(UTC)", PageContext.Localization.GetTextFormatted("Current_Time", YafServices.DateTime.FormatTime(DateTime.UtcNow))) : PageContext.Localization.GetTextFormatted("Current_Time", YafServices.DateTime.FormatTime(DateTime.UtcNow));

      if (Mession.LastVisit != DateTime.MinValue)
      {
        this.TimeLastVisit.Visible = true;
        this.TimeLastVisit.Text = string.Format(
                                         "{0}(UTC)", 
                                         this.PageContext.Localization.GetTextFormatted(
                                                                       "last_visit", 
                                                                       YafServices.DateTime.FormatDateTime(Mession.LastVisit)));
      }
      else
      {
        this.TimeLastVisit.Visible = false;
      }

      if (this.PageContext.UnreadPrivate > 0)
      {
        this.UnreadMsgs.Visible = true;
        this.UnreadMsgs.NavigateUrl = YafBuildLink.GetLink(ForumPages.cp_pm);
        if (this.PageContext.UnreadPrivate == 1)
        {
            this.UnreadMsgs.Text = this.PageContext.Localization.GetTextFormatted("unread1", PageContext.UnreadPrivate);
        }
        else
        {
            this.UnreadMsgs.Text = this.PageContext.Localization.GetTextFormatted("unread0", PageContext.UnreadPrivate);
        }
      }
    }
  }
}