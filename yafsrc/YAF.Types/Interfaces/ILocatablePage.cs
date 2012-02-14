/* YetAnotherForum.NET
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
namespace YAF.Types.Interfaces
{
  /// <summary>
  /// The i locatable page.
  /// </summary>
  public interface ILocatablePage
  {
    #region Properties

    /// <summary>
    ///   Gets a value indicating whether IsAdminPage.
    /// </summary>
    bool IsAdminPage { get; }

    /// <summary>
    ///   Gets a value indicating whether IsHostAdminOnly.
    /// </summary>
    bool IsHostAdminOnly { get; }

    /// <summary>
    /// Gets a value indicating whether IsRegisteredPage.
    /// </summary>
    bool IsRegisteredPage { get; }

    /// <summary>
    ///   Gets PageName.
    /// </summary>
    string PageName { get; }

    #endregion
  }
}