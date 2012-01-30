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

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The forum welcome.
  /// </summary>
  public partial class ForumWelcome : BaseUserControl
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ForumWelcome" /> class.
    /// </summary>
    public ForumWelcome()
    {
      this.PreRender += this.ForumWelcome_PreRender;
    }

    #endregion

    #region Methods

    /// <summary>
    /// The forum welcome_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumWelcome_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
      this.TimeNow.Text = this.GetTextFormatted(
        "Current_Time", this.Get<IDateTime>().FormatTime(DateTime.UtcNow));

    	var lastVisit = this.Get<IYafSession>().LastVisit;

      if (lastVisit.HasValue && lastVisit.Value != DateTime.MinValue)
      {
        this.TimeLastVisit.Visible = true;
        this.TimeLastVisit.Text = this.GetTextFormatted("last_visit", this.Get<IDateTime>().FormatDateTime(lastVisit.Value));
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
          this.UnreadMsgs.Text = this.GetTextFormatted(
            "unread1", this.PageContext.UnreadPrivate);
        }
        else
        {
          this.UnreadMsgs.Text = this.GetTextFormatted(
            "unread0", this.PageContext.UnreadPrivate);
        }
      }
    }

    #endregion
  }
}