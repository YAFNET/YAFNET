/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Björnar Henden
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

namespace YAF.Core.Services.Twitter
{
    using YAF.Types;
    using YAF.Types.Objects;
    using YAF.Utils;
    using YAF.Utils.Extensions;

    /// <summary>
    /// Twitter Class to publish, delete, retrieve tweets and Send Direct Messages via twitter.com
    /// </summary>
    public class TweetAPI
    {
        /// <summary>
        /// The authorized and authenticated oAuth token
        /// </summary>
        private readonly OAuthTwitter oAuth;

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetAPI"/> class. 
        /// Create, Update, retrieve, delete tweets(status messages) using this class
        /// </summary>
        /// <param name="_oauth">
        /// An authorized and authenticated oAuth token
        /// </param>
        public TweetAPI(OAuthTwitter _oauth)
        {
            this.oAuth = _oauth;
        }

        /// <summary>
        /// The Response Forumat for the oAuth Request
        /// </summary>
        public enum ResponseFormat
        {
            /// <summary>
            /// json Format Type
            /// </summary>
            json,

            /// <summary>
            /// xml Format Type
            /// </summary>
            xml
        }

        #region Show:ID

        /// <summary>
        /// Returns a single status, specified by the id parameter below. The status's author will be returned inline.
        /// This does not require authentication as long as the status is not protected
        /// This is a rate limited call
        /// </summary>
        /// <param name="response_format">The format in which you want twitter to respond</param>
        /// <param name="statusid">The numerical ID of the desired status.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you dont want to pass any optional parameters</param>
        /// <returns>Response string from twitter in user selected format</returns>
        public string ShowByID([NotNull] ResponseFormat response_format, [NotNull] string statusid, [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                OAuthTwitter.Method.GET,
                "http://api.twitter.com/1/statuses/show/{0}.{1}".FormatWith(statusid, response_format.ToString()),
                optionalparameters);
        }

        #endregion

        #region Update

        /// <summary>
        /// Updates the authenticating user's status. A status update with text identical 
        /// to the authenticating user's current status will be ignored to prevent duplicates.
        /// Authentication is required and this call is not rate limited
        /// </summary>
        /// <param name="reponse_format">
        /// The format in which you want twitter to respond
        /// </param>
        /// <param name="tweet_message">
        /// The text of your status update, up to 140 characters.
        /// </param>
        /// <param name="optionalparameters">
        /// Any optional paramters you want to pass
        /// </param>
        /// <returns>
        /// Response string from twitter in user selected format 
        /// </returns>
        public string UpdateStatus([NotNull] ResponseFormat reponse_format, [NotNull] string tweet_message, [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                OAuthTwitter.Method.POST,
                "http://api.twitter.com/1/statuses/update.{0}".FormatWith(reponse_format.ToString()),
                "status={0}{1}".FormatWith(tweet_message, optionalparameters));
        }

        #endregion

        #region Direct Message

        /// <summary>
        /// Send a Direct Message to the Specifc User Screen Name
        /// </summary>
        /// <param name="reponse_format">
        /// The format in which you want twitter to respond
        /// </param>
        /// <param name="screen_name">
        /// The user_id of the user to send the Message to.
        /// </param>
        /// <param name="message">
        /// The text of your message, up to 140 characters.
        /// </param>
        /// <returns>
        /// Response string from twitter in user selected format 
        /// </returns>
        public string SendDirectMessage([NotNull] ResponseFormat reponse_format, [NotNull] string screen_name, [NotNull] string message)
        {
            return this.oAuth.OAuthWebRequest(
                OAuthTwitter.Method.POST,
                "http://api.twitter.com/1/direct_messages/new.{0}".FormatWith(reponse_format.ToString()),
                "screen_name={0}&text={1}".FormatWith(screen_name, message));
        }

        /// <summary>
        /// Send a Direct Message to the Specifc User Name
        /// </summary>
        /// <param name="reponse_format">
        /// The format in which you want twitter to respond
        /// </param>
        /// <param name="user_id">
        /// The user_id of the user to send the Message to.
        /// </param>
        /// <param name="message">
        /// The text of your message, up to 140 characters.
        /// </param>
        /// <returns>
        /// Response string from twitter in user selected format 
        /// </returns>
        public string SendDirectMessage([NotNull] ResponseFormat reponse_format, [NotNull] int user_id, [NotNull] string message)
        {
            return this.oAuth.OAuthWebRequest(
                OAuthTwitter.Method.POST,
                "http://api.twitter.com/1/direct_messages/new.{0}".FormatWith(reponse_format.ToString()),
                "user_id={0}&text={1}".FormatWith(user_id, message));
        }

        #endregion

        #region Destroy:Id

        /// <summary>
        /// Destroys the status specified by the required ID parameter.In other words deletes the specified tweet
        /// Requires authentication, and rate limited is false
        /// </summary>
        /// <param name="response_format">The format in which you want twitter to respond</param>
        /// <param name="statusid">The numerical ID of the desired status.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you dont want to pass any optional parameters</param>
        /// <returns>Response string from twitter in user selected format</returns>
        public string DestroyById([NotNull] ResponseFormat response_format, [NotNull] string statusid, [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                OAuthTwitter.Method.POST,
                "http://api.twitter.com/1/statuses/destroy/{0}.{1}".FormatWith(statusid, response_format.ToString()),
                optionalparameters);
        }

        #endregion

        #region Retweet:Id

        /// <summary>
        /// Retweets a tweet. Returns the original tweet with retweet details embedded.
        /// Does not require authentication, and rate limited is false
        /// </summary>
        /// <param name="response_format">The format in which you want twitter to respond</param>
        /// <param name="statusid">The numerical ID of the desired status.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you dont want to pass any optional parameters</param>
        /// <returns>Response string from twitter in user selected format</returns>
        public string RetweetById([NotNull] ResponseFormat response_format, [NotNull] string statusid, [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                OAuthTwitter.Method.POST,
                "http://api.twitter.com/1/statuses/retweet/{0}.{1}".FormatWith(statusid, response_format.ToString()),
                optionalparameters);
        }

        #endregion

        #region Show Retweets:Id

        /// <summary>
        /// Returns up to 100 of the first retweets of a given tweet.
        /// Does not require authentication, and rate limited is false
        /// </summary>
        /// <param name="response_format">The format in which you want twitter to respond</param>
        /// <param name="statusid">The numerical ID of the desired status.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you dont want to pass any optional parameters</param>
        /// <returns>Response string from twitter in user selected format</returns>
        public string ShowRetweetsById([NotNull] ResponseFormat response_format, [NotNull] string statusid, [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                OAuthTwitter.Method.GET,
                "http://api.twitter.com/1/statuses/retweets/{0}.{1}".FormatWith(statusid, response_format.ToString()),
                optionalparameters);
        }

        #endregion

        #region Show Retweeted By:Id

        /// <summary>
        /// Show user objects of up to 100 members who retweeted the status.
        /// Requires authentication, and rate limited
        /// </summary>
        /// <param name="response_format">The format in which you want twitter to respond</param>
        /// <param name="statusid">The numerical ID of the desired status.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you dont want to pass any optional parameters</param>
        /// <returns>Response string from twitter in user selected format</returns>
        public string ShowRetweetedbyById([NotNull] ResponseFormat response_format, [NotNull] string statusid, [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                OAuthTwitter.Method.GET,
                "http://api.twitter.com/1/statuses/{0}/retweeted_by.{1}".FormatWith(statusid, response_format.ToString()),
                optionalparameters);
        }

        #endregion

        #region Get Current Twitter User Profile

        /// <summary>
        /// Get The Twitter User Profile of the Current User
        /// </summary>
        /// <returns>
        /// Returns the TwitterUser Profile
        /// </returns>
        public TwitterUser GetUser()
        {
            return
                this.oAuth.OAuthWebRequest(OAuthTwitter.Method.GET, "http://api.twitter.com/1/account/verify_credentials.json", string.Empty).FromJson<TwitterUser>();
        }

        #endregion
    }
}
