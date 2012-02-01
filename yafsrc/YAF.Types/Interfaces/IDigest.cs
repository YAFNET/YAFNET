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
  /// <summary>
  /// The digest interface
  /// </summary>
  public interface IDigest
  {
    #region Public Methods

      /// <summary>
      /// The get digest html.
      /// </summary>
      /// <param name="userId">The user id.</param>
      /// <param name="boardId">The board id.</param>
      /// <param name="showErrors">if set to <c>true</c> [show errors].</param>
      /// <returns>
      /// The get digest html.
      /// </returns>
    string GetDigestHtml(int userId, int boardId, bool showErrors = false);

    /// <summary>
    /// The get digest url.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <returns>
    /// The get digest url.
    /// </returns>
    string GetDigestUrl(int userId, int boardId);

    /// <summary>
    /// The get digest url.
    /// </summary>
    /// <param name="userId">
    /// The user id.
    /// </param>
    /// <param name="boardId">
    /// The board id.
    /// </param>
    /// <param name="showErrors">
    /// Show digest generation errors 
    /// </param>
    /// <returns>
    /// The get digest url.
    /// </returns>
    string GetDigestUrl(int userId, int boardId, bool showErrors);

    /// <summary>
    /// Sends the digest html to the email/name specified.
    /// </summary>
    /// <param name="digestHtml">
    /// The digest html.
    /// </param>
    /// <param name="forumName">
    /// The forum name.
    /// </param>
    /// <param name="toEmail">
    /// The to email.
    /// </param>
    /// <param name="toName">
    /// The to name.
    /// </param>
    /// <param name="sendQueued">
    /// The send queued.
    /// </param>
    void SendDigest(
      [NotNull] string digestHtml, 
      [NotNull] string forumName, 
      [NotNull] string toEmail, 
      [CanBeNull] string toName, 
      bool sendQueued);

    #endregion
  }
}