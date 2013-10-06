/* Yet Another Forum.NET
 * Copyright (C) 2006-2013 Jaben Cargman
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

  using YAF.Core.Services;
  using YAF.Types;
  using YAF.Types.Constants;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The i permissions extensions.
  /// </summary>
  public static class IPermissionsExtensions
  {
    #region Public Methods

    /// <summary>
    /// The check.
    /// </summary>
    /// <param name="permissions">
    /// The permissions.
    /// </param>
    /// <param name="permission">
    /// The permission.
    /// </param>
    /// <returns>
    /// The check.
    /// </returns>
    public static bool Check([NotNull] this IPermissions permissions, int permission)
    {
      CodeContracts.VerifyNotNull(permissions, "permissions");

      return permissions.Check((ViewPermissions)permission);
    }

    /// <summary>
    /// The handle request.
    /// </summary>
    /// <param name="permissions">
    /// The permissions.
    /// </param>
    /// <param name="permission">
    /// The permission.
    /// </param>
    public static void HandleRequest([NotNull] this IPermissions permissions, int permission)
    {
      CodeContracts.VerifyNotNull(permissions, "permissions");

      permissions.HandleRequest((ViewPermissions)permission);
    }

    #endregion
  }
}