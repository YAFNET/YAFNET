/* Yet Another Forum.net
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
namespace YAF.Core.Services
{
  #region Using

    using System;
    using System.Security.Cryptography;
    using System.Text;

    using YAF.Classes;
    using YAF.Core;
    using YAF.Types;
    using YAF.Types.Interfaces;
    using YAF.Utils;

  #endregion

  /// <summary>
  /// The YAF Irkoo Reputation Service.
  /// </summary>
  public class YafIrkoo
  {
    #region Constants and Fields

    /// <summary>
    ///   The md5
    /// </summary>
    private static readonly MD5 md5 = new MD5CryptoServiceProvider();

    #endregion

    #region Public Methods

    /// <summary>
    /// the Irkoo Javascript code.
    /// </summary>
    /// <returns>
    /// The irk js code.
    /// </returns>
    public static string IrkJsCode()
    {
      return
        @"var __irk_site = '{0}'; {1}var __irk_msg_not_authorized = '{2}';var __irk_msg_min_vote_up = '{3}';var __irk_msg_min_vote_down = '{4}';var __irk_msg_self = '{5}';(function () {{var irk = document.createElement('script'); irk.type = 'text/javascript'; irk.async = true; 
                irk.src = document.location.protocol + '//z.irkoo.com/irk.js'; var s = document.getElementsByTagName('script')[0];
                s.parentNode.insertBefore(irk, s);}})();"
          .FormatWith(
            YafContext.Current.Get<YafBoardSettings>().IrkooSiteID, 
            (!YafContext.Current.IsGuest)
               ? CreateIrkooCode(
                 YafContext.Current.PageUserID, 
                 YafContext.Current.PageUserName,
                 YafContext.Current.Get<YafBoardSettings>().IrkooSecretKey)
               : string.Empty, 
            YafContext.Current.Get<ILocalization>().GetText("IRKOO", "NOT_AUTHORIZED"), 
            YafContext.Current.Get<ILocalization>().GetText("IRKOO", "MIN_VOTE_UP"), 
            YafContext.Current.Get<ILocalization>().GetText("IRKOO", "MIN_VOTE_DOWN"), 
            YafContext.Current.Get<ILocalization>().GetText("IRKOO", "SELF"));
    }

    /// <summary>
    /// The rating html.
    /// </summary>
    /// <param name="userID">
    /// The User ID.
    /// </param>
    /// <returns>
    /// The Irkoo rating html, if Irkoo Service is enabled. Otherwise returns null.
    /// </returns>
    public static string IrkRating([NotNull] object userID)
    {
      CodeContracts.ArgumentNotNull(userID, "UserID");

        return (YafContext.Current.Get<YafBoardSettings>().EnableIrkoo &&
                (!YafContext.Current.IsGuest || YafContext.Current.Get<YafBoardSettings>().AllowGuestsViewReputation))
                   ? @"<small class='irk-rating' data-author-id='{0}'
                style='position:relative; top:-0.5em' title='{1}'></small>"
                         .FormatWith(
                             userID.ToString(),
                             YafContext.Current.Get<ILocalization>().GetText("IRKOO", "USER_REPUTATION"))
                   : string.Empty;
    }

    /// <summary>
    /// The vote html.
    /// </summary>
    /// <param name="messageID">
    /// The Message ID.
    /// </param>
    /// <param name="userID">
    /// The User ID.
    /// </param>
    /// <returns>
    /// The Irkoo vote html, if Irkoo Service is enabled. Otherwise returns null.
    /// </returns>
    public static string IrkVote([NotNull] object messageID, [NotNull] object userID)
    {
      CodeContracts.ArgumentNotNull(messageID, "MessageID");
      CodeContracts.ArgumentNotNull(userID, "UserID");

        return YafContext.Current.Get<YafBoardSettings>().EnableIrkoo
                   ? @"<div class='irk-vote' data-msg-id='{0}'
                data-author-id='{1}'>
                <img class='irk-down' src='http://z.irkoo.com/pix.gif' alt='{2}' title='{2}' />
                <span class='irk-count'>0</span>
                <img class='irk-up' src='http://z.irkoo.com/pix.gif' alt='{3}' title='{3}' />
                </div>"
                         .FormatWith(
                             messageID.ToString(),
                             userID.ToString(),
                             YafContext.Current.Get<ILocalization>().GetText("IRKOO", "VOTE_DOWN"),
                             YafContext.Current.Get<ILocalization>().GetText("IRKOO", "VOTE_UP"))
                   : string.Empty;
    }

    #endregion

    #region Methods

    /// <summary>
    /// Creates the irkoo code.
    /// </summary>
    /// <param name="id">
    /// The id.
    /// </param>
    /// <param name="name">
    /// The name.
    /// </param>
    /// <param name="secret">
    /// The secret.
    /// </param>
    /// <returns>
    /// The create irkoo code.
    /// </returns>
    private static string CreateIrkooCode(int id, [NotNull] string name, [NotNull] string secret)
    {
      CodeContracts.ArgumentNotNull(name, "name");
      CodeContracts.ArgumentNotNull(secret, "secret");

      string hash = MD5HexDigest(secret + MD5HexDigest(id.ToString()) + MD5HexDigest(name));

      // escape certain special characters
      string nameEsc = name.Replace("\\", "\\\\").Replace("'", "\\'").Replace("<", "\\<").Replace(">", "\\>");
      return "var __irk_user = {{id:'{0}', name:'{1}', hash:'{2}'}};".FormatWith(id, nameEsc, hash);
    }

    /// <summary>
    /// Ms the d5 hex digest.
    /// </summary>
    /// <param name="data">
    /// The data.
    /// </param>
    /// <returns>
    /// The m d 5 hex digest.
    /// </returns>
    [NotNull]
    private static string MD5HexDigest([NotNull] string data)
    {
      CodeContracts.ArgumentNotNull(data, "data");

      byte[] buffer = Encoding.UTF8.GetBytes(data);
      byte[] hash = md5.ComputeHash(buffer);
      return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
    }

    #endregion
  }
}