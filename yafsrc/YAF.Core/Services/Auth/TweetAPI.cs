/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2020 Ingo Herbote
 * https://www.yetanotherforum.net/
 * 
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * https://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services.Auth
{
    using System.Collections.Generic;

    using ServiceStack;

    using YAF.Types;
    using YAF.Types.Objects;

    /// <summary>
    /// Twitter Class to publish, delete, retrieve tweets and Send Direct Messages via twitter.com
    /// </summary>
    public class TweetAPI
    {
        /// <summary>
        /// The authorized and authenticated OAUTH token
        /// </summary>
        private readonly OAuthTwitter oAuth;

        /// <summary>
        /// Initializes a new instance of the <see cref="TweetAPI"/> class. 
        /// Create, Update, retrieve, delete tweets(status messages) using this class
        /// </summary>
        /// <param name="_oauth">
        /// An authorized and authenticated OAUTH token
        /// </param>
        public TweetAPI(OAuthTwitter _oauth)
        {
            this.oAuth = _oauth;
        }

        /// <summary>
        /// The Response Format for the OAUTH Request
        /// </summary>
        public enum ResponseFormat
        {
            /// <summary>
            /// JSON Format Type
            /// </summary>
            json,

            /// <summary>
            /// xml Format Type
            /// </summary>
            xml
        }

        #region User Lookup

        /// <summary>
        /// Returns the user info
        /// </summary>
        /// <param name="userNames">Name of the users.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you don't want to pass any optional parameters</param>
        /// <returns>
        /// Response string from twitter in user selected format
        /// </returns>
        public string UsersLookupJson(
            [NotNull] string userNames,
            [CanBeNull] string optionalparameters = null)
        {
            return this.oAuth.OAuthWebRequest(
                AuthUtilities.Method.GET,
                $"https://api.twitter.com/1.1/users/lookup.json?screen_name={userNames}",
                optionalparameters);
        }

        /// <summary>
        /// Returns the user info
        /// </summary>
        /// <param name="response_format">The format in which you want twitter to respond</param>
        /// <param name="userNames">Name of the users.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you don't want to pass any optional parameters</param>
        /// <returns>
        /// Response string from twitter in user selected format
        /// </returns>
        public List<TwitterUser> UsersLookup(
            [NotNull] ResponseFormat response_format,
            [NotNull] string userNames,
            [CanBeNull] string optionalparameters = null)
        {
            return this.oAuth.OAuthWebRequest(
                AuthUtilities.Method.GET,
                string.Format(
                    "https://api.twitter.com/1.1/users/lookup.{1}?screen_name={0}",
                    userNames,
                        response_format.ToString()),
                optionalparameters).FromJson<List<TwitterUser>>();
        }

        #endregion

        #region Show:ID

        /// <summary>
        /// Returns a single status, specified by the id parameter below. The status's author will be returned inline.
        /// This does not require authentication as long as the status is not protected
        /// This is a rate limited call
        /// </summary>
        /// <param name="response_format">The format in which you want twitter to respond</param>
        /// <param name="statusid">The numerical ID of the desired status.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you don't want to pass any optional parameters</param>
        /// <returns>Response string from twitter in user selected format</returns>
        public string ShowByID(
            [NotNull] ResponseFormat response_format,
            [NotNull] string statusid,
            [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                AuthUtilities.Method.GET,
                $"https://api.twitter.com/1.1/statuses/show/{statusid}.{response_format.ToString()}",
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
        /// Any optional parameters you want to pass
        /// </param>
        /// <returns>
        /// Response string from twitter in user selected format 
        /// </returns>
        public string UpdateStatus(
            [NotNull] ResponseFormat reponse_format,
            [NotNull] string tweet_message,
            [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                AuthUtilities.Method.POST,
                $"https://api.twitter.com/1.1/statuses/update.{reponse_format.ToString()}",
                $"status={tweet_message}{optionalparameters}");
        }

        #endregion

        #region Direct Message

        /// <summary>
        /// Send a Direct Message to the Specific User Screen Name
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
        public string SendDirectMessage(
            [NotNull] ResponseFormat reponse_format,
            [NotNull] string screen_name,
            [NotNull] string message)
        {
            return this.oAuth.OAuthWebRequest(
                AuthUtilities.Method.POST,
                $"https://api.twitter.com/1.1/direct_messages/new.{reponse_format.ToString()}",
                $"screen_name={screen_name}&text={message}");
        }

        /// <summary>
        /// Send a Direct Message to the Specific User Name
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
        public string SendDirectMessage(
            [NotNull] ResponseFormat reponse_format,
            [NotNull] int user_id,
            [NotNull] string message)
        {
            return this.oAuth.OAuthWebRequest(
                AuthUtilities.Method.POST,
                $"https://api.twitter.com/1.1/direct_messages/new.{reponse_format.ToString()}",
                $"user_id={user_id}&text={message}");
        }

        #endregion

        #region Destroy:Id

        /// <summary>
        /// Destroys the status specified by the required ID parameter.In other words deletes the specified tweet
        /// Requires authentication, and rate limited is false
        /// </summary>
        /// <param name="response_format">The format in which you want twitter to respond</param>
        /// <param name="statusid">The numerical ID of the desired status.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you don't want to pass any optional parameters</param>
        /// <returns>Response string from twitter in user selected format</returns>
        public string DestroyById(
            [NotNull] ResponseFormat response_format,
            [NotNull] string statusid,
            [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                AuthUtilities.Method.POST,
                $"https://api.twitter.com/1.1/statuses/destroy/{statusid}.{response_format.ToString()}",
                optionalparameters);
        }

        #endregion

        #region Retweet:Id

        /// <summary>
        /// Re-tweets a tweet. Returns the original tweet with re-tweet details embedded.
        /// Does not require authentication, and rate limited is false
        /// </summary>
        /// <param name="response_format">The format in which you want twitter to respond</param>
        /// <param name="statusid">The numerical ID of the desired status.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you don't want to pass any optional parameters</param>
        /// <returns>Response string from twitter in user selected format</returns>
        public string RetweetById(
            [NotNull] ResponseFormat response_format,
            [NotNull] string statusid,
            [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                AuthUtilities.Method.POST,
                $"https://api.twitter.com/1.1/statuses/retweet/{statusid}.{response_format.ToString()}",
                optionalparameters);
        }

        #endregion

        #region Show Retweets:Id

        /// <summary>
        /// Returns up to 100 of the first RETWEETs of a given tweet.
        /// Does not require authentication, and rate limited is false
        /// </summary>
        /// <param name="response_format">The format in which you want twitter to respond</param>
        /// <param name="statusid">The numerical ID of the desired status.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you don't want to pass any optional parameters</param>
        /// <returns>Response string from twitter in user selected format</returns>
        public string ShowRetweetsById(
            [NotNull] ResponseFormat response_format,
            [NotNull] string statusid,
            [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                AuthUtilities.Method.GET,
                $"https://api.twitter.com/1.1/statuses/retweets/{statusid}.{response_format.ToString()}",
                optionalparameters);
        }

        #endregion

        #region Show Retweeted By:Id

        /// <summary>
        /// Show user objects of up to 100 members who re-tweeted the status.
        /// Requires authentication, and rate limited
        /// </summary>
        /// <param name="response_format">The format in which you want twitter to respond</param>
        /// <param name="statusid">The numerical ID of the desired status.</param>
        /// <param name="optionalparameters">Any other optional parameters.Use an empty string if you don't want to pass any optional parameters</param>
        /// <returns>Response string from twitter in user selected format</returns>
        public string ShowRetweetedbyById(
            [NotNull] ResponseFormat response_format,
            [NotNull] string statusid,
            [NotNull] string optionalparameters)
        {
            return this.oAuth.OAuthWebRequest(
                AuthUtilities.Method.GET,
                $"https://api.twitter.com/1.1/statuses/{statusid}/retweeted_by.{response_format.ToString()}",
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
                this.oAuth.OAuthWebRequest(
                    AuthUtilities.Method.GET,
                    "https://api.twitter.com/1.1/account/verify_credentials.json",
                    string.Empty).FromJson<TwitterUser>();
        }

        #endregion
    }
}