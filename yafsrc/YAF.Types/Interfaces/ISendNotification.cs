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
    using System.Web.Security;

    /// <summary>
    /// The SendNotification Interface
    /// </summary>
    public interface ISendNotification
    {
        /// <summary>
        /// Sends Notifications to Moderators that Message Needs Approval
        /// </summary>
        /// <param name="forumId">
        /// The forum id.
        /// </param>
        /// <param name="newMessageId">
        /// The new message id.
        /// </param>
        void ToModeratorsThatMessageNeedsApproval(int forumId, int newMessageId);

        /// <summary>
        /// Sends Notifications to Moderators that a Message was Reported
        /// </summary>
        /// <param name="pageForumID">
        /// The page Forum ID.
        /// </param>
        /// <param name="reportedMessageId">
        /// The reported message id.
        /// </param>
        /// <param name="reporter">
        /// The reporter.
        /// </param>
        /// <param name="reportText">
        /// The report Text.
        /// </param>
        void ToModeratorsThatMessageWasReported(int pageForumID, int reportedMessageId, int reporter, string reportText);

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

        /// <summary>
        /// Send an Email to the Newly Created User with
        /// his Account Info (Pass, Security Question and Answer)
        /// </summary>
        /// <param name="user">
        /// The user.
        /// </param>
        /// <param name="pass">
        /// The pass.
        /// </param>
        /// <param name="securityAnswer">
        /// The security answer.
        /// </param>
        /// <param name="templateName">
        /// The template Name.
        /// </param>
        void SendRegistrationNotificationToUser(
            [NotNull] MembershipUser user, [NotNull] string pass, [NotNull] string securityAnswer, string templateName);

        /// <summary>
        /// Sends notification that the User was awarded with a Medal
        /// </summary>
        /// <param name="toUserId">To user id.</param>
        /// <param name="medalName">Name of the medal.</param>
        void ToUserWithNewMedal([NotNull] int toUserId, [NotNull] string medalName);
    }
}