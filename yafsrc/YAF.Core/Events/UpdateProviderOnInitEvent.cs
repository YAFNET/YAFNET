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
  using YAF.Types;
  using YAF.Types.Attributes;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The update provider on init event.
  /// </summary>
  [ExportService(ServiceLifetimeScope.InstancePerScope)]
  public class UpdateProviderOnInitEvent : IHandleEvent<ForumPageInitEvent>
  {
    #region Constants and Fields

    /// <summary>
    /// The _board settings.
    /// </summary>
    private readonly YafBoardSettings _boardSettings;

    /// <summary>
    /// The _membership provider.
    /// </summary>
    private readonly MembershipProvider _membershipProvider;

    /// <summary>
    /// The _role provider.
    /// </summary>
    private readonly RoleProvider _roleProvider;

    #endregion

    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="UpdateProviderOnInitEvent"/> class.
    /// </summary>
    /// <param name="membershipProvider">
    /// The membership provider.
    /// </param>
    /// <param name="roleProvider">
    /// The role provider.
    /// </param>
    /// <param name="boardSettings">
    /// The board settings.
    /// </param>
    public UpdateProviderOnInitEvent([NotNull] MembershipProvider membershipProvider, [NotNull] RoleProvider roleProvider, [NotNull] YafBoardSettings boardSettings)
    {
      this._membershipProvider = membershipProvider;
      this._roleProvider = roleProvider;
      this._boardSettings = boardSettings;
    }

    #endregion

    #region Properties

    /// <summary>
    ///   Gets Order.
    /// </summary>
    public int Order
    {
      get
      {
        return 1000;
      }
    }

    #endregion

    #region Implemented Interfaces

    #region IHandleEvent<ForumPageInitEvent>

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle([NotNull] ForumPageInitEvent @event)
    {
      // initialize the providers...
      if (!this._membershipProvider.ApplicationName.Equals(this._boardSettings.MembershipAppName))
      {
        this._membershipProvider.ApplicationName = this._boardSettings.MembershipAppName;
      }

      if (!this._roleProvider.ApplicationName.Equals(this._boardSettings.RolesAppName))
      {
        this._roleProvider.ApplicationName = this._boardSettings.RolesAppName;
      }
    }

    #endregion

    #endregion
  }
}