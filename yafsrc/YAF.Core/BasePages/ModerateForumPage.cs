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

namespace YAF.Core
{
  #region Using

  using System;

  using YAF.Types;
  using YAF.Types.Extensions;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The moderate forum page.
  /// </summary>
  public class ModerateForumPage : ForumPage
  {
    #region Constructors and Destructors

      /// <summary>
      /// Initializes a new instance of the <see cref="ModerateForumPage"/> class.
      /// </summary>
      public ModerateForumPage()
      : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="ModerateForumPage"/> class.
    /// </summary>
    /// <param name="transPage">
    /// The trans page.
    /// </param>
    public ModerateForumPage([CanBeNull] string transPage)
      : base(transPage)
    {
    }

    #endregion

    #region Properties

    /// <summary>
    /// Gets PageName.
    /// </summary>
    public override string PageName
    {
      get
      {
        return "moderate_{0}".FormatWith(base.PageName);
      }
    }

    #endregion

    #region Methods

    /// <summary>
    /// The page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    protected void Page_Load([NotNull] object sender, [NotNull] EventArgs e)
    {
      // Only moderators are allowed here
      if (!this.PageContext.ForumModeratorAccess || !this.PageContext.IsModeratorInAnyForum)
      {
        YafBuildLink.AccessDenied();
      }
    }

    #endregion
  }
}