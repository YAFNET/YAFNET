/* Yet Another Forum.NET
 * Copyright (C) 2003-2005 Bjørnar Henden
 * Copyright (C) 2006-2013 Jaben Cargman
 * Copyright (C) 2014-2018 Ingo Herbote
 * http://www.yetanotherforum.net/
 *
 * Licensed to the Apache Software Foundation (ASF) under one
 * or more contributor license agreements.  See the NOTICE file
 * distributed with this work for additional information
 * regarding copyright ownership.  The ASF licenses this file
 * to you under the Apache License, Version 2.0 (the
 * "License"); you may not use this file except in compliance
 * with the License.  You may obtain a copy of the License at

 * http://www.apache.org/licenses/LICENSE-2.0

 * Unless required by applicable law or agreed to in writing,
 * software distributed under the License is distributed on an
 * "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY
 * KIND, either express or implied.  See the License for the
 * specific language governing permissions and limitations
 * under the License.
 */

namespace YAF.Core.Services
{
    using System;
    using System.Net;
    using System.Web;

    using YAF.Classes;
    using YAF.Core.Services.CheckForSpam;
    using YAF.Types;
    using YAF.Types.Extensions;
    using YAF.Types.Interfaces;

    /// <summary>
    /// User and Content Spam Checking
    /// </summary>
    public class YafSpamCheck
    {
        /// <summary>
        /// Check a Post for SPAM against the BlogSpam.NET API or AKISMET Service
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="ipAddress">The IP address.</param>
        /// <param name="postMessage">The post message.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// Returns if Post is SPAM or not
        /// </returns>
        public bool CheckPostForSpam(
            [NotNull] string userName,
            [NotNull] string ipAddress,
            [NotNull] string postMessage,
            [CanBeNull] string emailAddress,
            out string result)
        {
            result = string.Empty;

            if (YafContext.Current.Get<YafBoardSettings>().SpamServiceType.Equals(0))
            {
                return false;
            }

            if (YafContext.Current.CurrentUserData.NumPosts
                >= YafContext.Current.Get<YafBoardSettings>().IgnoreSpamWordCheckPostCount)
            {
                return false;
            }

            switch (YafContext.Current.Get<YafBoardSettings>().SpamServiceType)
            {
                case 1:
                    return this.CheckWithBlogSpam(userName, postMessage, ipAddress, out result);
                case 2:
                    {
                        return YafContext.Current.Get<YafBoardSettings>().AkismetApiKey.IsSet()
                               && this.CheckWithAkismet(userName, postMessage, ipAddress, out result);
                    }

                case 3:
                    {
                        return YafContext.Current.Get<ISpamWordCheck>().CheckForSpamWord(postMessage, out result);
                    }
            }

            return false;
        }

        /// <summary>
        /// Check a User (Bot) against the StopForumSpam, BotScout Service or both
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="emailAddress">The email address.</param>
        /// <param name="ipAddress">The IP address.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// Returns if Post is SPAM or not
        /// </returns>
        public bool CheckUserForSpamBot([NotNull]string userName, [CanBeNull]string emailAddress, [NotNull]string ipAddress, out string result)
        {
            // Check internal
            var internalCheck = new YafInternalCheck();

            var isInternalFoundBot = internalCheck.IsBot(ipAddress, emailAddress, userName, out result);

            if (isInternalFoundBot)
            {
                return true;
            }

            if (YafContext.Current.Get<YafBoardSettings>().BotSpamServiceType.Equals(0))
            {
                return false;
            }

            switch (YafContext.Current.Get<YafBoardSettings>().BotSpamServiceType)
            {
                case 1:
                    {
                        var stopForumSpam = new StopForumSpam();

                        return stopForumSpam.IsBot(ipAddress, emailAddress, userName, out result);
                    }

                case 2:
                    {
                        if (YafContext.Current.Get<YafBoardSettings>().BotScoutApiKey.IsSet())
                        {
                            var botScout = new BotScout();

                            return botScout.IsBot(ipAddress, emailAddress, userName, out result);
                        }

                        // use StopForumSpam instead
                        var stopForumSpam = new StopForumSpam();

                        return stopForumSpam.IsBot(ipAddress, emailAddress, userName, out result);
                    }

                case 3:
                    {
                        // use StopForumSpam instead
                        var stopForumSpam = new StopForumSpam();

                        if (!YafContext.Current.Get<YafBoardSettings>().BotScoutApiKey.IsSet())
                        {
                            return stopForumSpam.IsBot(ipAddress, emailAddress, userName, out result);
                        }

                        var botScout = new BotScout();

                        return botScout.IsBot(ipAddress, emailAddress, userName)
                               && stopForumSpam.IsBot(ipAddress, emailAddress, userName, out result);
                    }

                case 4:
                    {
                        // use StopForumSpam instead
                        var stopForumSpam = new StopForumSpam();

                        if (!YafContext.Current.Get<YafBoardSettings>().BotScoutApiKey.IsSet())
                        {
                            return stopForumSpam.IsBot(ipAddress, emailAddress, userName, out result);
                        }

                        var botScout = new BotScout();

                        return botScout.IsBot(ipAddress, emailAddress, userName)
                               | stopForumSpam.IsBot(ipAddress, emailAddress, userName, out result);
                    }
            }

            return false;
        }

        /// <summary>
        /// Checks with blog spam.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="postMessage">The post message.</param>
        /// <param name="ipAddress">The IP Address.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// Returns if the Content or the User was flagged as Spam, or not
        /// </returns>
        private bool CheckWithBlogSpam([NotNull]string userName, [NotNull]string postMessage, [NotNull]string ipAddress, out string result)
        {
            var isLocal = YafContext.Current.Get<HttpRequestBase>().IsLocal;

            var whiteList = isLocal ? "whitelist=127.0.0.1" : string.Empty;

            try
            {
                return
                    BlogSpamNet.CommentIsSpam(
                        new BlogSpamComment
                        {
                            comment = postMessage,
                            ip = ipAddress,
                            agent = YafContext.Current.Get<HttpRequestBase>().UserAgent,
                            name = userName,
                            options = whiteList,
                        },
                        true,
                        out result);
            }
            catch (Exception ex)
            {
                YafContext.Current.Get<ILogger>().Error(ex, "Error while Checking for Spam via BlogSpam");

                result = string.Empty;
                return false;
            }
        }

        /// <summary>
        /// Checks with AKISMET.
        /// </summary>
        /// <param name="userName">Name of the user.</param>
        /// <param name="postMessage">The post message.</param>
        /// <param name="ipAddress">The IP Address.</param>
        /// <param name="result">The result.</param>
        /// <returns>
        /// Returns if the Content or the User was flagged as Spam, or not
        /// </returns>
        private bool CheckWithAkismet(
            [NotNull] string userName,
            [NotNull] string postMessage,
            [NotNull] string ipAddress,
            out string result)
        {
            try
            {
                var service = new AkismetSpamClient(YafContext.Current.Get<YafBoardSettings>().AkismetApiKey, new Uri(BaseUrlBuilder.BaseUrl));

                return
                    service.CheckCommentForSpam(
                        new Comment(IPAddress.Parse(ipAddress), YafContext.Current.Get<HttpRequestBase>().UserAgent)
                            {
                                Content
                                    =
                                    postMessage,
                                Author
                                    =
                                    userName,
                            },
                        out result);
            }
            catch (Exception ex)
            {
                YafContext.Current.Get<ILogger>().Error(ex, "Error while Checking for Spam via BlogSpam");

                result = string.Empty;
                return false;
            }
        }
    }
}
