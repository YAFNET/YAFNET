/* Yet Another Forum.net
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
  #region Using

  using YAF.Types.Attributes;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The provider settings module.
  /// </summary>
  [YafModule("Provider Settings Module", "Tiny Gecko", 1)]
  public class ProviderSettingsForumModule : BaseForumModule, IHandleEvent<ForumPageInitEvent>
  {
    #region Properties

    /// <summary>
    ///   Gets PageContext.
    /// </summary>
    public YafContext PageContext
    {
      get
      {
        return YafContext.Current;
      }
    }

    /// <summary>
    /// The initialization function.
    /// </summary>
    public override void Init()
    {
      
    }

    #endregion

    #region Implementation of IHandleEvent<in ForumPageInitEvent>

    /// <summary>
    /// Gets Order.
    /// </summary>
    public int Order
    {
      get
      {
        return 1000;
      }
    }

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle(ForumPageInitEvent @event)
    {
      // initialize the providers...
      if (!this.PageContext.CurrentMembership.ApplicationName.Equals(this.PageContext.BoardSettings.MembershipAppName))
      {
        this.PageContext.CurrentMembership.ApplicationName = this.PageContext.BoardSettings.MembershipAppName;
      }

      if (!this.PageContext.CurrentRoles.ApplicationName.Equals(this.PageContext.BoardSettings.RolesAppName))
      {
        this.PageContext.CurrentRoles.ApplicationName = this.PageContext.BoardSettings.RolesAppName;
      }
    }

    #endregion
  }
}