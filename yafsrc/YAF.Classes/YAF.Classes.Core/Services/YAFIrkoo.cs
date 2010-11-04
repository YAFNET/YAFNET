using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace YAF.Classes.Core.Services
{
    /// <summary>
    ///  The YAF Irkoo Reputation Service.
    /// </summary>
    public class YAFIrkoo
    {
        /// <summary>
        /// The md5
        /// </summary>
        private static readonly System.Security.Cryptography.MD5 md5 =
            new System.Security.Cryptography.MD5CryptoServiceProvider();

        /// <summary>
        /// Creates the irkoo code.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="name">The name.</param>
        /// <param name="secret">The secret.</param>
        /// <returns></returns>
        private static string CreateIrkooCode(int id, string name, string secret)
        {
            string hash = MD5HexDigest(secret + MD5HexDigest(id.ToString()) + MD5HexDigest(name));
            // escape certain special characters
            string nameEsc = name.Replace("\\", "\\\\").Replace("'", "\\'").Replace("<", "\\<").Replace(">", "\\>");
            return string.Format("var __irk_user = {{id:'{0}', name:'{1}', hash:'{2}'}};", id, nameEsc, hash);
        }

        /// <summary>
        /// Ms the d5 hex digest.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        private static string MD5HexDigest(string data)
        {
            byte[] buffer = System.Text.Encoding.UTF8.GetBytes(data);
            byte[] hash = md5.ComputeHash(buffer);
            return BitConverter.ToString(hash).Replace("-", string.Empty).ToLower();
        }

        /// <summary>
        /// the Irkoo Javascript code.
        /// </summary>
        public static string IrkJsCode()
        {
            return @"var __irk_site = '" + YafContext.Current.BoardSettings.IrkooSiteID + "'; " +
                ((!YafContext.Current.IsGuest) ? CreateIrkooCode(YafContext.Current.PageUserID, YafContext.Current.PageUserName, YafContext.Current.BoardSettings.IrkooSecretKey) : string.Empty) +
                "var __irk_msg_not_authorized = '" + YafContext.Current.Localization.GetText("IRKOO", "NOT_AUTHORIZED") + "';" +
                "var __irk_msg_min_vote_up = '" + YafContext.Current.Localization.GetText("IRKOO", "MIN_VOTE_UP") + "';" +
                "var __irk_msg_min_vote_down = '" + YafContext.Current.Localization.GetText("IRKOO", "MIN_VOTE_DOWN") + "';" +
                "var __irk_msg_self = '" + YafContext.Current.Localization.GetText("IRKOO", "SELF") + "';" +
                @"(function () {var irk = document.createElement('script'); irk.type = 'text/javascript'; irk.async = true; 
                irk.src = document.location.protocol + '//z.irkoo.com/irk.js'; var s = document.getElementsByTagName('script')[0];
                s.parentNode.insertBefore(irk, s);})();";
        }

        /// <summary>
        /// The vote html.
        /// </summary>
        /// <returns>
        /// The Irkoo vote html, if Irkoo Service is enabled. Otherwise returns null.
        /// </returns>
        public static string IrkVote(object MessageID, object UserID)
        {
            return (YafContext.Current.BoardSettings.EnableIrkoo)? string.Format(@"<div class='irk-vote' data-msg-id='{0}'
                data-author-id='{1}'>
                <img class='irk-down' src='http://z.irkoo.com/pix.gif' alt='' />
                <span class='irk-count'>0</span>
                <img class='irk-up' src='http://z.irkoo.com/pix.gif' alt='' />
                </div>", MessageID.ToString(), UserID.ToString()) : string.Empty;
        }

        /// <summary>
        /// The rating html.
        /// </summary>
        /// <returns>
        /// The Irkoo rating html, if Irkoo Service is enabled. Otherwise returns null.
        /// </returns>
        public static string IrkRating(object UserID)
        {
            return (YafContext.Current.BoardSettings.EnableIrkoo &&
                (!YafContext.Current.IsGuest || 
                YafContext.Current.BoardSettings.AllowGuestsViewReputation))? 
                string.Format(@"<small class='irk-rating' data-author-id='{0}'
                style='position:relative; top:-0.5em'></small>", UserID.ToString()) : string.Empty;
        }

    }
}
