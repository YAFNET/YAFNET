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
  using System.Data;

  public interface IBuddy
  {
    /// <summary>
    /// Adds a buddy request.
    /// </summary>
    /// <param name="ToUserID">
    /// the to user id.
    /// </param>
    /// <returns>
    /// The name of the second user + whether this request is approved or not. (This request
    /// is approved without the target user's approval if the target user has sent a buddy request
    /// to current user too or if the current user is already in the target user's buddy list.
    /// </returns>
    string[] AddRequest(int toUserID);

    /// <summary>
    /// Approves all buddy requests for the current user.
    /// </summary>
    /// <param name="Mutual">
    /// should the users be added to current user's buddy list too?
    /// </param>
    void ApproveAllRequests(bool mutual);

    /// <summary>
    /// Approves a buddy request.
    /// </summary>
    /// <param name="toUserID">
    /// the to user id.
    /// </param>
    /// <param name="mutual">
    /// should the second user be added to current user's buddy list too?
    /// </param>
    /// <returns>
    /// The name of the second user.
    /// </returns>
    string ApproveRequest(int toUserID, bool mutual);

    /// <summary>
    /// Gets all the buddies of the current user.
    /// </summary>
    /// <returns>
    /// A <see cref="DataTable"/> of all buddies.
    /// </returns>
    DataTable All();

    /// <summary>
    /// Clears the buddies cache for the current user.
    /// </summary>
    /// <param name="userID">
    /// The User ID.
    /// </param>
    void ClearCache(int userID);

    /// <summary>
    /// Denies all buddy requests for the current user.
    /// </summary>
    void DenyAllRequests();

    /// <summary>
    /// Denies a buddy request.
    /// </summary>
    /// <param name="toUserID">
    /// The to user id.
    /// </param>
    /// <returns>
    /// the name of the second user.
    /// </returns>
    string DenyRequest(int toUserID);

    /// <summary>
    /// Gets all the buddies for the specified user.
    /// </summary>
    /// <param name="userID">
    /// The user id.
    /// </param>
    /// <returns>
    /// a <see cref="DataTable"/> of all buddies.
    /// </returns>
    DataTable GetForUser(int userID);

    /// <summary>
    /// determines if the "<paramref name="buddyUserID"/>" and current user are buddies.
    /// </summary>
    /// <param name="buddyUserID">
    /// The Buddy User ID.
    /// </param>
    /// <param name="approved">
    /// Just look into approved buddies?
    /// </param>
    /// <returns>
    /// true if they are buddies, <see langword="false"/> if not.
    /// </returns>
    bool IsBuddy(int buddyUserID, bool approved);

    /// <summary>
    /// Removes the "<paramref name="toUserID"/>" from current user's buddy list.
    /// </summary>
    /// <param name="toUserID">
    /// The to user id.
    /// </param>
    /// <returns>
    /// The name of the second user.
    /// </returns>
    string Remove(int toUserID);
  }
}