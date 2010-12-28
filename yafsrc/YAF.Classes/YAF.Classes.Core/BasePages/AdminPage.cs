/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
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
namespace YAF.Core
{
  using System;

  using YAF.Utils;

  /// <summary>
  /// Admin page with extra security. All admin pages need to be derived from this base class.
  /// </summary>
  public class AdminPage : ForumPage
  {
    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPage"/> class. 
    /// Creates the Administration page.
    /// </summary>
    public AdminPage()
      : this(null)
    {
    }

    /// <summary>
    /// Initializes a new instance of the <see cref="AdminPage"/> class.
    /// </summary>
    /// <param name="transPage">
    /// The trans page.
    /// </param>
    public AdminPage(string transPage)
      : base(transPage)
    {
      IsAdminPage = true;
      Load += this.AdminPage_Load;
    }

    /// <summary>
    /// The admin page_ load.
    /// </summary>
    /// <param name="sender">
    /// The sender.
    /// </param>
    /// <param name="e">
    /// The e.
    /// </param>
    private void AdminPage_Load(object sender, EventArgs e)
    {
      if (!PageContext.IsAdmin)
      {
        YafBuildLink.AccessDenied();
      }
    }
  }
}