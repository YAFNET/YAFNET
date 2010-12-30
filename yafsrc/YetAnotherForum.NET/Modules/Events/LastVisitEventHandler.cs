/* YetAnotherForum.NET
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
namespace YAF.Modules
{
  #region Using

  using System;

  using YAF.Core;
  using YAF.Types;
  using YAF.Types.EventProxies;
  using YAF.Types.Interfaces;

  #endregion

  /// <summary>
  /// The last visit handler.
  /// </summary>
  public class LastVisitEventHandler : IHandleEvent<ForumPagePreLoadEvent>
  {
    #region Constructors and Destructors

    /// <summary>
    /// Initializes a new instance of the <see cref="LastVisitEventHandler"/> class.
    /// </summary>
    /// <param name="yafSession">
    /// The yaf session.
    /// </param>
    public LastVisitEventHandler([NotNull] IYafSession yafSession)
    {
      this.YafSession = yafSession;
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

    /// <summary>
    /// Gets or sets YafSession.
    /// </summary>
    public IYafSession YafSession { get; set; }

    #endregion

    #region Implemented Interfaces

    #region IHandleEvent<ForumPagePreLoadEvent>

    /// <summary>
    /// The handle.
    /// </summary>
    /// <param name="event">
    /// The event.
    /// </param>
    public void Handle([NotNull] ForumPagePreLoadEvent @event)
    {
      if (!YafContext.Current.IsGuest && YafContext.Current.Page["PreviousVisit"] != DBNull.Value && !this.YafSession.HasLastVisit)
      {
        this.YafSession.LastVisit = Convert.ToDateTime(YafContext.Current.Page["PreviousVisit"]);
        this.YafSession.HasLastVisit = true;
      }
      else if (this.YafSession.LastVisit == DateTime.MinValue)
      {
        this.YafSession.LastVisit = DateTime.UtcNow;
      }
    }

    #endregion

    #endregion
  }
}