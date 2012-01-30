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
  using System.Collections;
  using System.Data;

  using YAF.Core;
  using YAF.Types;

  #endregion

  /// <summary>
  /// The forum moderator list.
  /// </summary>
  public partial class ForumModeratorList : BaseUserControl
  {
    #region Constructors and Destructors

    /// <summary>
    ///   Initializes a new instance of the <see cref = "ForumModeratorList" /> class.
    /// </summary>
    public ForumModeratorList()
    {
      this.PreRender += this.ForumModeratorList_PreRender;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Sets DataSource.
    /// </summary>
    public IEnumerable DataSource
    {
      set
      {
        this.ModeratorList.DataSource = value;
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The forum moderator list_ pre render.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void ForumModeratorList_PreRender([NotNull] object sender, [NotNull] EventArgs e)
    {
      if (((DataRow[])this.ModeratorList.DataSource).Length > 0)
      {
        // no need for the "blank dash"...
        this.BlankDash.Visible = false;
      }
    }

    #endregion
  }
}