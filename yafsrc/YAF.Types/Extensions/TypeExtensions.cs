/* Yet Another Forum.net
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
namespace YAF.Types.Extensions
{
  #region Using

    using System;
    using System.Linq;
    using System.Security.Permissions;
    using System.Security.Policy;

    using YAF.Types;

    #endregion

  /// <summary>
  /// The type extensions.
  /// </summary>
  public static class TypeExtensions
  {
    #region Public Methods

    /// <summary>
    /// The get signing key.
    /// </summary>
    /// <param name="sourceType">
    /// The source type.
    /// </param>
    /// <returns>
    /// </returns>
    public static StrongNamePublicKeyBlob GetSigningKey([NotNull] this Type sourceType)
    {
      CodeContracts.VerifyNotNull(sourceType, "sourceType");

      return sourceType.Assembly.Evidence.OfType<StrongName>().Select(t => t.PublicKey).FirstOrDefault();
    }

    #endregion
  }
}