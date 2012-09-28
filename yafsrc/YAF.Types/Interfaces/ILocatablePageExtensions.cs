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
namespace YAF.Types.Interfaces
{
  #region Using

    using System;
    using System.Collections.Generic;
    using System.Linq;

    #endregion

  /// <summary>
  /// The locatable page extension.
  /// </summary>
  public static class ILocatablePageExtensions
  {
    #region Public Methods

    /// <summary>
    /// The get page.
    /// </summary>
    /// <param name="locatablePages">
    /// The locatable pages.
    /// </param>
    /// <param name="pageName">
    /// The page name.
    /// </param>
    /// <returns>
    /// </returns>
    public static ILocatablePage GetPage([NotNull] this IEnumerable<ILocatablePage> locatablePages, [NotNull] string pageName)
    {
      return
        locatablePages.FirstOrDefault(
          p => String.Compare(p.PageName, pageName, StringComparison.CurrentCultureIgnoreCase) == 0);
    }

    #endregion
  }
}