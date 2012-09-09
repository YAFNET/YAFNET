/* Yet Another Forum.net
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

  using System.Web.Security;

  using YAF.Classes;
  using YAF.Types.Extensions;
  using YAF.Types.Interfaces;
  using YAF.Utils;

  #endregion

  /// <summary>
  /// The current membership provider.
  /// </summary>
  public class CurrentMembershipProvider : IReadOnlyProvider<MembershipProvider>
  {
    #region Properties

    /// <summary>
    ///   The instance.
    /// </summary>
    /// <returns>
    /// </returns>
    public MembershipProvider Instance
    {
      get
      {
        if (Config.MembershipProvider.IsSet() && Membership.Providers[Config.MembershipProvider] != null)
        {
          return Membership.Providers[Config.MembershipProvider];
        }

        // return default membership provider
        return Membership.Provider;
      }
    }

    #endregion
  }
}