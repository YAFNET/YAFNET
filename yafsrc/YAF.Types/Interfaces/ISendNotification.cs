/* Yet Another Forum.NET
 * Copyright (C) 2006-2011 Jaben Cargman
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
  public interface ISendNotification
  {
    /// <summary>
    /// The to moderators that message needs approval.
    /// </summary>
    /// <param name="forumId">
    /// The forum id.
    /// </param>
    /// <param name="newMessageId">
    /// The new message id.
    /// </param>
    void ToModeratorsThatMessageNeedsApproval(int forumId, int newMessageId);

    /// <summary>
    /// Sends notification about new PM in user's inbox.
    /// </summary>
    /// <param name="toUserId">
    /// User supposed to receive notification about new PM.
    /// </param>
    /// <param name="subject">
    /// Subject of PM user is notified about.
    /// </param>
    void ToPrivateMessageRecipient(int toUserId, [NotNull] string subject);

    /// <summary>
    /// The to watching users.
    /// </summary>
    /// <param name="newMessageId">
    /// The new message id.
    /// </param>
    void ToWatchingUsers(int newMessageId);
  }
}